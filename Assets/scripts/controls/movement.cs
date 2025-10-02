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

    public float currentXRotation;

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
    public AudioSource AS;

    public Animator Shot_effect;
    public Animator AttackDisplay;
    public Animator crosshairDisplay;

    public GameObject playerCamera;

    [Header("data -- custom")]
    public AT_base attack;

    [Header("defaults")]
    public AT_base D_Attack;
    public RuntimeAnimatorController D_AttackDisplay;
    public RuntimeAnimatorController D_crosshair;

    public List<Pickup> interactables;


    void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        AS = GetComponent<AudioSource>();

        // curser
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // load attack
        attack.load(this);
    }

    void Update() {

        #region movement
        if (CanMove) {

            // get the desired force
            Vector3 targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down") * moveSpeed;
            targetVelocity += transform.right * eevee.input.CheckAxis("right", "left") * moveSpeed;

            if (SmoothMovement && Control){ // apply it naturally
                rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref Velocity, MovementSmoothing);
            } else { // apply it forcefully
                rb.AddForce(targetVelocity);
            }

            // camera rotation
            HandleMouse();
        }
        #endregion

        #region attack
        if (eevee.input.Grab("Attack")) {
            attack.attack(this);
        }
        #endregion
    }

    // stolen from the amazing A curr, thank you queen.
    void HandleMouse() {
        float mouseX = Input.GetAxis("Mouse X") * RT_Modifier;

        transform.Rotate(Vector3.up * mouseX);

        currentXRotation = Mathf.Clamp(currentXRotation, -90, 90);

        playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
    }

    // reset to default values
    public void Reset() {
        AttackDisplay.runtimeAnimatorController = D_AttackDisplay;
        crosshairDisplay.runtimeAnimatorController = D_crosshair;
        attack = D_Attack;
    }

    // switch attack
    public void switchAttack(AT_base newAttack) {
        Reset();

        attack = newAttack;
        attack.load(this);
    }

    #region dev

    #endregion

    #region IEunmerators

    #endregion
}
