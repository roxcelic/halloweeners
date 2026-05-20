using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

public class NEN_retro : NEN_base {
    protected override void Start() {
        rb = transform.GetComponent<Rigidbody>();
    }

    public virtual void begin() {
        StartCoroutine(movement());
    }

    public override IEnumerator movement() {
        yield return new WaitUntil(() => playerController.mainPlayer != null);
        while (true) {
            if (canMove) {
                Vector3 direction = (playerController.mainPlayer.transform.position - transform.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z);

                rb.linearVelocity = direction * moveSpeed;
                Debug.Log($"distance to player is {Vector3.Distance(transform.position, playerController.mainPlayer.transform.position)}");

                yield return 0;
            }
        }
    }
}