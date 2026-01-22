using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using player.utils;
using player.move;

namespace player.abil {
    public static class access {
        /// <summery> This is a util to allow me to wait before running code without making a custom IEnumerator for each <summery>
        public static IEnumerator waitForTime(this playerController pc, System.Action input, float time) {
            yield return new WaitForSeconds(time);
            input();
        }

        /// <summery> This is a util similar to waitForTime which waits for a specified key to be pressed <summery>
        /// at some point id like to make one for general any input at all
        public static IEnumerator waitForInput(this playerController pc, System.Action input, string key = "interact") {
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => eevee.input.Grab(key));
            input();
        }

        /// <summery> every frame while a key is held run some code <summery>
        /// halt : bool
        ///     remove input while held
        /// halfVerScale: bool
        ///     Half the verticle scale of the player while the button is held
        /// This is mostly used for the dash i beleive
        public static IEnumerator whileHeld(this playerController pc, System.Action input, string key, bool halt = false, bool halfVerScale = false, System.Action before = null, System.Action after = null) {
            if (halfVerScale) {
                Debug.Log("gy");
                pc.transform.localScale = new Vector3(pc.transform.localScale.x,pc. transform.localScale.y / 2, pc.transform.localScale.z);
            }
            if (halt) pc.CanMove = false;
            if (before != null) before();

            while(eevee.input.Check(key)) {
                input();
                yield return 0;
            }
            
            if (halt) pc.CanMove = true;
            if (halfVerScale) pc.transform.localScale = new Vector3(pc.transform.localScale.x, pc.transform.localScale.y * 2, pc.transform.localScale.z);
            if (after != null) after();
        }

        /// <summery> moves the player forwards constantly </summery>
        /// This also decays the players velocity by a set amount which is slower than when not sliding
        public static IEnumerator slide(this playerController pc) {
            if (pc.sliding) yield break;

            pc.col.transform.localScale = new Vector3(1, 1 - pc.crouchDistance, 1);
            
            // rb.AddForce(-transform.up * jumpForce * 2f); // thow them down twin

            pc.CanMove = false;
            pc.addVel.update = false;
            pc.sliding = true;

            Vector3 slideForce = pc.rb.linearVelocity;
            float targetPos = pc.transform.position.y - pc.crouchDistance;
            pc.addVel.vel = 0;

            while(eevee.input.Check("Slam") && pc.isGrounded(1, 2f) && (!eevee.input.Check("Jump") || pc.jumpCount <= 0)) {
                pc.transform.position = Vector3.Lerp(pc.transform.position, new Vector3(
                    pc.transform.position.x,
                    targetPos,
                    pc.transform.position.z
                ), Time.deltaTime * 5f);

                pc.rb.linearVelocity = slideForce;

                // x velocity clamp
                if (slideForce.x > pc.stopSpeed) slideForce = new Vector3(slideForce.x - pc.slideDecay, slideForce.y, slideForce.z);
                else if (slideForce.x < -pc.stopSpeed) slideForce = new Vector3(slideForce.x + pc.slideDecay, slideForce.y, slideForce.z);
                else slideForce = new Vector3(0, slideForce.y, slideForce.z);

                // z velocity clamp
                if (slideForce.z > pc.stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y, slideForce.z - pc.slideDecay);
                else if (slideForce.z < -pc.stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y, slideForce.z + pc.slideDecay);
                else slideForce = new Vector3(slideForce.x, slideForce.y, 0);

                slideForce = new Vector3(slideForce.x, 0, slideForce.z);

                yield return 0;
            }

            pc.rb.linearVelocity = new Vector3();

            pc.addVel.update = true;
            pc.addVel.vel = slideForce.magnitude / 2;
            pc.CanMove = true;
            pc.sliding = false;

            pc.col.transform.localScale = new Vector3(1, 1, 1);
            pc.transform.position += new Vector3(0, pc.crouchDistance, 0);

            if (eevee.input.Check("Jump") && pc.jumpCount > 0) {
               pc. addVel.AddForce(5);
                pc.jump();
            }
        }

        /// <summery> adds force downwards on the player while removing their ability to move </summery>
        /// if the slam button is still pressed after its finished move the player into a slide
        public static IEnumerator slam(this playerController pc) {
            Debug.Log("starting slam");
            pc.CanMove = false;

            Vector3 hldVel = pc.rb.linearVelocity;
            pc.rb.linearVelocity = new Vector3();

            float outForce = 0f;
            while(!pc.isGrounded() && (!eevee.input.Check("Jump") || pc.jumpCount <= 0)) {
                outForce = pc.rb.linearVelocity.y;
                pc.rb.AddForce(-pc.transform.up * pc.jumpForce * 2f);
                yield return new WaitForSeconds(0.1f);
            }

            if (eevee.input.Check("Jump")) {
                pc.jump();
                pc.CanMove = true;
            } else {
                pc.addVel.AddForce(Mathf.Abs(outForce));

                if (eevee.input.Check("Slam")) {
                    pc.rb.linearVelocity = hldVel;

                    pc.StartCoroutine(pc.slide());
                } else pc.CanMove = true;
            }
        }

        /// <summery> similar to slide, yet this freezes the player in air and stops time <summery>
        public static IEnumerator dasher(this playerController pc, Vector3 target) {
            Time.timeScale = 0f;
            pc.CanMove = false;
            pc.canDash = false;

            while (Vector3.Distance(pc.transform.position, target) > 1f) {
                Vector3 difference = target - pc.transform.position;
                difference = new Vector3(difference.x, 0, difference.z) * 0.5f;

                pc.transform.position = Vector3.Lerp(pc.transform.position, target + difference, Time.fixedDeltaTime * pc.dashSpeed);
                yield return 0;
            }

            Time.timeScale = GS.live.state.gameSpeed;
            pc.CanMove = true;

            pc.addVel.AddForce(pc.outDashForce);

            yield return new WaitForSeconds(pc.dashDelay);
            pc.canDash = true;
        }

        /// <summery> a very basic jump delay </summery>
        /// might be better to make this use a `waitForTime` function to assist in readability while this method makes it easier for me to seperate and keep track of
        public static IEnumerator allowNextJump(this playerController pc) {
            pc.canResetJump = false;
            yield return new WaitForSeconds(0.1f);
            pc.canResetJump = true;        
        }
    }
}