using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class spawnWalkableEffect : MonoBehaviour {
    [Header("components")]
    public GameObject Effect;

    [Header("config")]
    [Range(0.025f, 5f)] public float spawnDuration; // the wait between spawns
    [Range(0.025f, 1f)] public float spawnDistance; // how far the player has to move without being registered as not moving
    [Range(0.025f, 1f)] public float moveCheckInterval; // how long until the program checks if the players moving
    public Vector3 spawnOffset = new Vector3(0, 0.1f, 0);

    [Header("data")]
    public bool isPlayerOnPlatform = false;
    public bool isPlayerMoving = false;
    private GameObject player;

    /// <summery> start the coroutine to grab the player then set its state to active </summery>
    void Start() {
        // set defaults
        isPlayerOnPlatform = false;

        // start wait
        StartCoroutine(waitForPlayer());
    }

    /// <summery> if the player touches the platform then set active </summery>
    void OnCollisionStay(Collision col) {if (col.gameObject == player) isPlayerOnPlatform = true;}

    /// <summery> if the player leaves the platform then set active </summery>
    void OnCollisionExit(Collision col) {if (col.gameObject == player) isPlayerOnPlatform = false;}

    /// <summery> grab the player </summery>
    public IEnumerator waitForPlayer() {
        yield return new WaitUntil(() => playerController.mainPlayer != null);
        player = playerController.mainPlayer.transform.gameObject;
        
        // move to the next coroutine, i know i could have put it in one but i like to be organised
        StartCoroutine(followPlayer());
        StartCoroutine(trackPlayerMovement());
    }

    /// <summery> tracks the players movement </summery>
    public IEnumerator trackPlayerMovement() {
        Vector3 lastCheck = player.transform.position;
        while (true) {
            isPlayerMoving = Vector3.Distance(lastCheck, player.transform.position) > spawnDistance;

            lastCheck = player.transform.position;

            yield return new WaitForSeconds(moveCheckInterval);
        }
    }

    /// <summery> follow the player spawning the effect based on the config </summery>
    public IEnumerator followPlayer() {
        while(true) {
            yield return new WaitUntil(() => isPlayerOnPlatform && isPlayerMoving);

            // find spawn positionv
            if (Physics.Raycast(player.transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, Mathf.Infinity)) Instantiate(Effect, hit.point + spawnOffset, Effect.transform.rotation);
            else Debug.Log("genuinly what the fuck did you do :sob:");

            // loop
            yield return new WaitForSeconds(spawnDuration);
        }
    }
}