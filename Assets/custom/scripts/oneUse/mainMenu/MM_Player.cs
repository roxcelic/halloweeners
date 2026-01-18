using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

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
        if(canMoveCamera) HandleMouse();
        if (eevee.input.Grab("Attack") || eevee.input.Grab("interact")) openMenu();
    }

    // wonder where i stole this from.......
    public void openMenu() {
        RaycastHit hit;
        MM_Term tmpHld;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
            if ((tmpHld = hit.collider.transform.GetComponent<MM_Term>()) != null) tmpHld.open();
        }
    }

    public override void HandleMouse() {
        if (CanMove) {
            float mouseX = Input.GetAxisRaw("Mouse X") * (RT_Modifier * getData.config().sense);
            float mouseY = Input.GetAxisRaw("Mouse Y") * (RT_Modifier * getData.config().sense) / 2;

            transform.Rotate(Vector3.up * mouseX);
        } else {
            float mouseX = Input.GetAxisRaw("Mouse X") * (RT_Modifier * getData.config().sense);
            float mouseY = Input.GetAxisRaw("Mouse Y") * (RT_Modifier * getData.config().sense) / 2;

            camera.Rotate(Vector3.up * mouseX);
        }

    }
}