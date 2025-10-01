using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class movement : MonoBehaviour {

    [Header("data")]
    [Range(0, .3f)] public float MovementSmoothing = .05f;
    public float moveSpeed = 5f;
	
    public Vector3 Velocity = Vector3.zero;

    // movement bools
    public bool SmoothMovement = true;
    public bool Control = false;
    public bool CanMove = true;

    [Header("rotation")]
    [Range(0f, 15f)] public float RT_Modifier = 5f;
    public Vector3 camera;

    [Header("componenets")]
    public Rigidbody rb;
    public Collider col;
    public Animator Shot_effect;

    void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // curser
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {

        #region movement
        // physical movement
        if (CanMove) {

            if (SmoothMovement && Control){
                Vector3 targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down") * moveSpeed;
                targetVelocity += transform.right * eevee.input.CheckAxis("right", "left") * moveSpeed;

                rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref Velocity, MovementSmoothing);
            } else {
                rb.AddForce(new Vector3(eevee.input.CheckAxis("right", "left") * moveSpeed, 0f, eevee.input.CheckAxis("up", "down") * moveSpeed));
            }

            // rotation
            float mouseX = Input.GetAxis("Mouse X") * RT_Modifier;

            transform.Rotate(Vector3.up * mouseX);
        }
        #endregion

        #region attack
        if (eevee.input.Grab("Attack") && !Shot_effect.GetCurrentAnimatorStateInfo(0).IsName("flash")) {
            Shot_effect.Play("flash");
        }
        #endregion
    }

    #region IEunmerators
    #endregion
}
