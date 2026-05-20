using UnityEngine;
using UnityEngine.AI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using player.health;

public class NEN_neMeRelinque : NEN_base {
    [Header("ne me relinque settings")]
    [Range(0, 10f)] public float subjectCount = 5f; // the amount of subjects it can have   
    [Range(0, 25f)] public float range = 25f; // the range at which is can get a subject
    public float timerDuration = 14f;
    public int damage = 1;

    [Header("components")]
    public Animator timerAnimator; // the animator component on the timer

    [Header("Data")]
    public List<EN_base> subjects = new List<EN_base>();

    [Header("spawn conf")]
    [Range(10, 25f)] public float eyeline = 15f;
    public  float checkDelay = 0.2f;
    public float risenHeight = 7.5f;
    public GameObject select;

    /// <summery> gathers a set of random subjects within the given range </summery>
    public List<EN_base> gatherSubjects() {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, range, transform.forward, Mathf.Infinity);
        hit = hit.Where(c => c.collider.gameObject.tag == "Enemy" && c.collider.gameObject != transform.gameObject).ToArray();

        if (hit.Length > 0) {
            foreach(RaycastHit enemy in hit) {

                EN_base selected = enemy.collider.GetComponent<EN_base>();
                if (selected != null && !subjects.Contains(selected) && subjects.Count < subjectCount) {
                    subjects.Add(selected);
                    spawnSelected(selected.transform);
                    selected.allowDeath = false;
                } else Debug.Log($"unable to teach {enemy.collider.gameObject.name} due to an issue with its base: {selected}");

            }
        } else {
            Debug.Log("hit nothing");
        }

        return new List<EN_base>();
    }

    private bool checkForPlayer() {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, eyeline, transform.forward, eyeline);

        Debug.Log(hit.Length);

        hit = hit.Where(c => c.collider.gameObject.tag == "PlayerB" && c.collider.gameObject != transform.gameObject).ToArray();

        Debug.Log(hit.Length);

        if (hit.Length > 0) {
            return true;
        } else {
            return false;
        }
    }

    private GameObject spawnSelected(Transform target) {
        GameObject SP_select = Instantiate(select);
        SP_select.transform.parent = target;
        SP_select.transform.localPosition = new Vector3();
        return SP_select;
    }

    #region  basic AI

    public override void begin() {
        StartCoroutine(waitForPLayer());
    }

    public void coBegin() {
        StartCoroutine(waitForScholars());
        StartCoroutine(countDown());
    }

    #endregion

    #region  coroutines
    public IEnumerator waitForPLayer() {
        yield return new WaitUntil(() => GS.live.state.loaded);
        
        bool foundPlayer = false;
        while(!foundPlayer) {
            foundPlayer = checkForPlayer();
            yield return new WaitForSeconds(checkDelay);
        }     

        // rise
        Vector3 startPos = transform.position;
        float endPos = startPos.y + risenHeight;

        while (Vector3.Distance(transform.position, new Vector3(transform.position.x, endPos, transform.position.z)) > 0.01f) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, endPos, transform.position.z), Time.deltaTime * 5f);
            yield return 0;
        }

        transform.position = new Vector3(transform.position.x, endPos, transform.position.z);
        timerAnimator.transform.gameObject.SetActive(true);

        coBegin();
    }

    public IEnumerator waitForScholars() {
        while (subjects.Count < subjectCount) {
            gatherSubjects();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator countDown() {
        yield return new WaitUntil(() => timerAnimator != null);

        int CUR = 7;

        while (!self.dead) {
            timerAnimator.Play($"{CUR}-{CUR - 1}");

            CUR--;
            if (CUR == 0) {
                List <EN_base> aliveEnemys = subjects.Where(subject => !subject.dead).ToList();

                if (aliveEnemys.Count > 0) {
                    playerController.mainPlayer.DealDamage(damage * aliveEnemys.Count, transform);

                    // revive all
                    foreach(EN_base enemy in subjects) {
                        enemy.Revive();
                        Debug.Log($"revived {enemy}");
                    }

                    CUR = 7;
                } else {
                    foreach(EN_base enemy in subjects) {
                        enemy.allowDeath = true;
                        enemy.Die();
                    };
                    self.Die();
                }
            }
            yield return new WaitForSeconds(timerDuration / 7);
        }
    }
    #endregion
}