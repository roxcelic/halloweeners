using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/*
    - This is pretty much just a way to seperate the dash from the reset of the player,
            I had to like, remake the dash for my intention of adding a dashblock like celeste
*/
namespace player.abil.dash {
    public static class access {
        // a simple variable class to hold data like what can be dashed through
        public static class var {
            // what blocks can be dashed into without limiting the end position
            public static List<string> dashableBlocks = new List<string>{
                "dashable Block"
            };

        }

        /// <summery> This function basically checks if the player can dash and then starts the `dasher` coroutine </summery>
        public static void dash(this playerController pc) {
            if (!pc.canDash) return;
            string dashData = ""; // DEV :: this is used while developing in order to log the information about its dash
            
            // find the dash direction
                // use the camera since in order to make a more dyanamic movement being able to dash up and down would be increadibly important, i wonder how itll feel with using the controls for its 2D direction rather than the way the camera is facing alone.
            Vector3 dashDirection = new Vector3();
                dashDirection = pc.camera.forward * eevee.input.CheckAxis("up", "down"); // get the direction from the forward
                dashDirection += pc.camera.right * eevee.input.CheckAxis("right", "left"); // get the direction from the right
            
            // if the input is 0,0,0 then use the cameras forwards
            if (dashDirection == new Vector3()) dashDirection = pc.camera.forward;
            
            dashData += $"\n dash direction: {dashDirection}"; // DEV :: adds the dash direction to the log

            // multiply the dash direction by the dash length and then add it to the current position
            Vector3 dashOutPosition = (dashDirection * pc.dashDistance) + pc.transform.position;

            dashData += $"\n dash out position: {dashOutPosition}, dash length: {pc.dashDistance}";

            // limit the dash using a small raycast
            if (Physics.Raycast(pc.transform.position, dashDirection, out RaycastHit hit, pc.dashDistance)){
                dashData += $"\n hit a block, blocktag: {hit.collider.gameObject.tag}, hit: {hit.point}, hit distance: {hit.distance}"; // DEV
                if (var.dashableBlocks.Contains(hit.collider.gameObject.tag)) {
                    // allow the player to dash unrestricted
                    dashData += $"\n\t the block is dashable"; // DEV
                } else {
                    // stop the dash
                    dashData += $"\n\t the block is not dashable, setting the dash out position to the hit position"; // DEV
                    dashOutPosition = hit.point;
                }
            }

            // start the dash
            dashData += $"\n the final dash out: playerpos: {pc.transform.position}; dashOutPos: {dashOutPosition }";
            pc.StartCoroutine(pc.dasher(dashOutPosition, dashDirection));

            // Debug.Log(dashData); // DEV :: this is what logs the dash data, delete these lines later twin
        }

        /// <summery> this is a small co-routine which will move the player kinda slowly </smmery>
        public static IEnumerator dasher(this playerController pc, Vector3 outPosition, Vector3 dir) {
            // stop time and playermovement
            pc.CanMove = false;
            pc.canDash = false;
            pc.isDashing = true;

            // store data
            float passedTime = 0f;
            
            // loop
            while (Vector3.Distance(pc.transform.position, outPosition) > 1f && pc.restrictions !=playerController.movementRestriction.dashBlock ) {
                // im not too sure what this does atm, its 2:53am sooo. I do know it stops it from slowing down at the end but idk why
                Vector3 difference = outPosition - pc.transform.position;
                difference = new Vector3(difference.x, difference.y, difference.z) * 0.5f;

                // actual dash movement
                passedTime += Time.fixedDeltaTime * pc.dashSpeed;
                pc.transform.position = Vector3.Lerp(pc.transform.position, outPosition + difference, passedTime);

                yield return 0;
            }

            // if the dash was broken because of the dashblock movement restriction then instantly replenish the dash restriction and break
            if (pc.restrictions == playerController.movementRestriction.dashBlock) {
                pc.CanMove = true;
                pc.canDash = true;
                pc.isDashing = false;
            } else {
                // or proceed as normal
                pc.CanMove = true;
                pc.isDashing = false;

               pc.addVel.AddForce(Vector3.Dot( pc.transform.forward, dir) > 0 ? pc.outDashForce : - pc.outDashForce);

                yield return new WaitForSeconds(pc.dashDelay);
                pc.canDash = true;
            }
        }
    }
}