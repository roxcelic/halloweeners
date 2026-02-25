using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

using player.abil;

namespace player.move {
    public static class access {
        /// <summery> This is what allows the player to look around and what not </summery>
        public static void HandleMouse(this playerController pc) {
            float mouseX = Input.GetAxisRaw("Mouse X") * (pc.RT_Modifier * getData.config().sense);
            float mouseY = Input.GetAxisRaw("Mouse Y") * (pc.RT_Modifier  * getData.config().sense) / 2;

            pc.transform.Rotate(Vector3.up * mouseX);

            pc.currentXRotation -= mouseY;
            pc.currentXRotation = Mathf.Clamp(pc.currentXRotation, -pc.cameraClamp, pc.cameraClamp);

            pc.camera.localRotation = Quaternion.Euler(pc.currentXRotation, 0f, 0f);
        }

        /// <summery> allows the player to add a set amount of velocity to the player </summery>
        /// it also clears the players Y velocity 
        public static void jump(this playerController pc) {
            if (pc.jumpCount <= 0 || !pc.canJump) return; // if cant jump then dont

            pc.rb.linearVelocity = new Vector3(pc.rb.linearVelocity.x, 0, pc.rb.linearVelocity.z);
            pc.rb.AddForce(pc.transform.up * pc.jumpForce);
            pc.jumpCount--;

            pc.StartCoroutine(pc.allowNextJump());
            
            pc.canJump = false;
            pc.StartCoroutine(pc.waitForTime(
                () => {pc.canJump = true;},
                0.25f
            ));
        }

        /// <summery> a check to see if the player is on a slop </summery>
        public static bool onSlope(this playerController pc, float multiplier = 1.1f, float distance = 0f) {
            if (Physics.Raycast(pc.transform.position, Vector3.down, out pc.slopeHit, distance == 0f ? Vector3.Distance(pc.transform.position, pc.groundCheck.position) * multiplier : distance)) {
                float angle = Vector3.Angle(Vector3.up, pc.slopeHit.normal);

                if (pc.slopeHit.collider.gameObject.tag == pc.rampTag) return false;
                return angle < pc.maxSlopAngle && angle != 0;
            }

            return false;
        }

        /// <summery> find force direction on slope </summery>
        public static Vector3 getSlopeModeDirection(this playerController pc, Vector3 moveDirection) {
            return Vector3.ProjectOnPlane(moveDirection, pc.slopeHit.normal).normalized;
        }
    }
}