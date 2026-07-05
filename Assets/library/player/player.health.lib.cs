using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using player.abil;

namespace player.health {
    public static class access {
                /// <summery> DealDamage </summery>
        /// this would typically apply a single point of damage unless i wanted to do a silksong and be horribly evil
        public static void DealDamage(this playerController pc, int damage = 1, Transform dealer = null, bool nockback = true, float nockbackForce = 1f) {
            if (pc.liveIftames > 0) return;

            damage = pc.loaded ? damage : 0;
            
            pc.health -= damage;
            pc.health = Math.Clamp(pc.health, 0, pc.maxHealth);
            pc.ScreenEffect.Play("hurt");

            if (pc.health <= 0) pc.Die();
            else {
                pc.AS.clip = pc.hurtsound;
                pc.AS.Play();
                // transform.GetComponent<cameraTilt>().shake(25, 2);
            }

            if (dealer != null && nockback) {
                pc.addVel.AddForce(-(nockbackForce));
            }

            pc.liveIftames += pc.maxIframes;

            // if (damage > 0) hud.displayText(profanities.Count > 0 ? profanities[UnityEngine.Random.Range(0, profanities.Count - 1)] : "owwwww", Color.red);
        }

        /// <summery> heal </summery>
        /// this would again typically heal a single point of health, but can be overriden by things like life steal
        public static void heal(this playerController pc, int damage = 1) {
            pc.health += damage;

            pc.health = Math.Clamp(pc.health, 0, pc.maxHealth);
        }

        /// <summery> die </summery>
        /// This originally just deleted the player but now it does one of two things
        ///     Check if the player has instant respawn turned on
        ///         if yes reload the scene
        ///         if no turn on the death screen and wait for an input
        public static void Die(this playerController pc) {
            if (save.getData.config().instantRespawn) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else {
                pc.deathScreen.SetActive(true);
                pc.StartCoroutine(pc.waitForInput(() => {
                    pc.deathScreen.transform.GetComponent<Animator>().Play("deathScreenClose");
                }));
            }
        }

        /// <summery> reset pos </summery>
        /// A nice little function to add the glitch effect to the screen and move the player back to where they where last safe
        public static void resetToSaftey(this playerController pc) {
            pc.ScreenEffect.Play("glitch");
            pc.transform.position = pc.lastSafePos;
            pc.CanMove = false;

            pc.StartCoroutine(pc.waitForTime(() => {
                pc.transform.position =pc. lastSafePos;
                pc.rb.linearVelocity = new Vector3();
                pc.addVel.vel = 0;
                pc.CanMove = true;
            }, 0.1f));
        }
    }
}