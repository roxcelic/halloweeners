using UnityEngine;
using UnityEngine.AI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NEN_neMeRelinque : NEN_base {
    [Header("ne me relinque settings")]
    [Range(0, 10f)] public float subjectCount = 5f; // the amount of subjects it can have   
    [Range(0, 25f)] public float range = 25f; // the range at which is can get a subject

    public List<EN_base> subjects = new List<EN_base>(); 

    /// <summery> gathers a set of random subjects within the given range </summery>
    public List<EN_base> gatherSubjects() {
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, range, transform.forward, Mathf.Infinity);
        hit = hit.Where(c => c.collider.gameObject.tag == "Enemy" && c.collider.gameObject != transform.gameObject).ToArray();

        if (hit.Length > 0) {
            foreach(RaycastHit enemy in hit) {

                EN_base selected = enemy.collider.GetComponent<EN_base>();
                if (selected != null && !subjects.Contains(selected) && subjects.Count < subjectCount) subjects.Add(selected);
                else Debug.Log($"unable to teach {enemy.collider.gameObject.name} due to an issue with its base: {selected}");

            }
        } else {
            Debug.Log("hit nothing");
        }

        return new List<EN_base>();
    }

    #region  basic AI

    public override void begin() {
        StartCoroutine(waitForScholars());
    }

    #endregion

    #region  coroutines
    public IEnumerator waitForScholars() {
        while (subjects.Count < subjectCount) {
            gatherSubjects();
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion
}