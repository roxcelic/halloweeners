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

    /// <summery> gathers a set of random subjects within the given range </summery>
    public List<EN_base> gatherSubjects() {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, range, transform.forward, Mathf.Infinity);
        hit = hit.Where(c => c.collider.gameObject.tag == "Enemy" && c.collider.gameObject != transform.gameObject).ToArray();

        if (hit.Length > 0) {
            foreach(RaycastHit enemy in hit) {

                EN_base selected = enemy.collider.GetComponent<EN_base>();
                if (selected != null && !subjects.Contains(selected) && subjects.Count < subjectCount) {
                    subjects.Add(selected);
                    selected.allowDeath = false;
                } else Debug.Log($"unable to teach {enemy.collider.gameObject.name} due to an issue with its base: {selected}");

            }
        } else {
            Debug.Log("hit nothing");
        }

        return new List<EN_base>();
    }

    #region  basic AI

    public override void begin() {
        StartCoroutine(waitForScholars());
        StartCoroutine(countDown());
    }

    #endregion

    #region  coroutines
    public IEnumerator waitForScholars() {
        while (subjects.Count < subjectCount) {
            gatherSubjects();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator countDown() {
        yield return new WaitUntil(() => timerAnimator != null);
        yield return new WaitUntil(() => GS.live.state.loaded);

        int CUR = 7;

        while (!self.dead) {
            timerAnimator.Play($"{CUR}-{CUR - 1}");

            CUR--;
            if (CUR == 0) {
                List <EN_base> aliveEnemys = subjects.Where(subject => !subject.dead).ToList();

                if (aliveEnemys.Count > 0) {
                    playerController.mainPlayer.DealDamage(damage * aliveEnemys.Count, transform);

                    // revive all
                    foreach(EN_base enemy in subjects) enemy.Revive();

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