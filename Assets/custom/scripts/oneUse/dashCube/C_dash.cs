using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

// sorryryryryryy its 4am fuckkkkkkkkkkkkkkkkkkkkkkkkkkk
//      -- and again what am i doing?????????????????????????????????????
public class C_dash : MonoBehaviour {
    [Header("config")]
    public float speed = 10f;
    public float outForce = 10f;

    Collider self;

    void Start() {
        self = transform.GetComponent<Collider>();
    }
    
    /// <summery> this should probably be called when i touch it silly style </summery>
    void OnCollisionEnter(Collision collision) {
       // if not the player or if not dashing do nothing
        if (collision.gameObject.tag != sys.var.config.playerTag || !playerController.mainPlayer.isDashing) return;

        // this is temporary, i need to make the players stop fuction take a movement restriction input so i can cache its everything yap yap yap yap
        playerController.mainPlayer.stop(playerController.movementRestriction.dashBlock);

        StartCoroutine(manualDash(playerController.mainPlayer));
    }

    public IEnumerator manualDash(playerController target) {
        // move the player forwards
        yield return 0;
        Debug.Log($"the direction the player is moving is: {target.camera.forward}");
        Vector3 dir = target.camera.forward;

        // loop until the player leaves
        while(isPlayerInBounds(target)) {
            target.transform.position = Vector3.Lerp(target.transform.position, target.transform.position + dir, Time.fixedDeltaTime * speed);

            yield return 0;
        }

        // finish the dash
        target.play(); // free the player
        target.jumpCount = target.maxJumpCount;
        if (eevee.input.Check("Jump")) target.addVel.AddForce(outForce);
    }

    private bool isPlayerInBounds(playerController target) {
        Vector3 direction = (transform.position - target.transform.position).normalized;
    
        return self.bounds.Contains(target.transform.position + direction);
    }
}