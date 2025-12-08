using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

public class NEN_base : MonoBehaviour {
    [Header("config")]
    public bool canMove = true;
    [Range(0, 5f)] public float pathCalculationDelay = 2.5f;
    [Range(0f, 100f)] public float moveSpeed;

    // comp
    [Header("components")]
    public EN_base self;
    public UnityEngine.AI.NavMeshAgent NV_Agent;

    protected virtual void Start() {
        // get components
        self = transform.GetComponent<EN_base>();
        NV_Agent = transform.GetComponent<NavMeshAgent>();

        NV_Agent.speed = moveSpeed;
    }

    public virtual void begin() {
        // start co routines
        StartCoroutine(movement());
    }

    /*
        CoRoutine to keep track of the player and if the player can move
    */
    public virtual IEnumerator movement() {
        Debug.Log("starting");
        while (true && !self.dead && NV_Agent.enabled) {
            NV_Agent.SetDestination(canMove ? playerController.mainPlayer.transform.position : transform.position);
            
            yield return new WaitForSeconds(pathCalculationDelay);

            float passedTime = 0f;
            bool cached = canMove;

            Debug.Log("moving");

            while (passedTime < pathCalculationDelay && cached == canMove) passedTime += Time.deltaTime;
        }
        Debug.Log("ending");
    }
}