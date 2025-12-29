using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class MM_Player : playerController {
    protected override void Start() {
        mainPlayer = this;
        
        // get the components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // velocity
        addVel = new movement.additionalVelocity(0, updateSpeed, true);
        StartCoroutine(addVel.start(rb));

        // curser
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    protected override void Update() {
        if (CanMove) {
            Vector3 targetVelocity = new Vector3();

            targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down") * moveSpeed;
            targetVelocity += transform.right * eevee.input.CheckAxis("right", "left") * moveSpeed;
            targetVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z) + addVel.getVelocity(this);

            if (SmoothMovement && Control){ // apply it naturally
                rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref Velocity, MovementSmoothing);
            } else { // apply it forcefully
                rb.AddForce(targetVelocity);
            }
        }

        // camera rotation
        HandleMouse();
        if (eevee.input.Grab("Attack")) openMenu();
    }

    // wonder where i stole this from.......
    public void openMenu() {
        RaycastHit hit;
        MM_Term tmpBrain;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
            if ((tmpBrain = hit.collider.transform.GetComponent<MM_Term>()) != null) tmpBrain.open();
        }
    }

    public override void HandleMouse() {
        if (CanMove) {
            float mouseX = Input.GetAxisRaw("Mouse X") * RT_Modifier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * RT_Modifier / 2;

            transform.Rotate(Vector3.up * mouseX);
        } else {
            float mouseX = Input.GetAxisRaw("Mouse X") * RT_Modifier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * RT_Modifier / 2;

            camera.Rotate(Vector3.up * mouseX);
        }

    }
}