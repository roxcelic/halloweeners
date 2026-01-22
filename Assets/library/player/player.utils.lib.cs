using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/**
* a simple utility library for the player
**/
namespace player.utils {
    public static class access {
        /// <summery> resets the players animators </summery>
        /// This is something i plan on phasing out as it adds an extra layer of complexity to the attacks which is un-needed as there will never be a point when the player doesnt have an attack loaded
        public static AT_base Reset(this playerController pc) {
            if (pc.attack == null) return null;
            
            pc.attack.unLoad(pc);
            AT_base hldatk = pc.attack;
            
            pc.AttackDisplay.runtimeAnimatorController = pc.D_AttackDisplay;
            pc.attack = null;

            return hldatk;
        }

        /// <summery> a util to allow an animation to ran the ability on the player </summery>
        public static void runAbility(this playerController pc) {
            pc.ability.use(pc);
            pc.abilityCharge.Play("idle");
        }

        /// <summery> allows the player to chain their main attack <summery>
        /// This function should call the 
        ///     attack.load
        ///     attack.unLoad
        /// functions to allow for things like stat editing when you use a weapon and switching the animators
        public static void switchAttack(this playerController pc, AT_base newAttack) {
            // Reset(); // should work fine without this
            if(pc.attack != null) pc.attack.unLoad(pc);

            pc.attack = GameObject.Instantiate(newAttack);
            
            pc.attack.load(pc);

            save.saveData currentSave = save.getData.viewSave();

            // save attack
                currentSave.currentAttack = pc.attack.name;
                currentSave.currentAttackData =pc. attack.attackData;

            save.getData.save(currentSave);
        }

        /// <summery> </summery>
        /// This is a really important function i recommend coming back to
        public static void ViewThoughts(this playerController pc) {
            RaycastHit hit;
            brain tmpBrain;

            if (Physics.Raycast(pc.transform.position, pc.camera.forward, out hit, Mathf.Infinity)) {
                if ((tmpBrain = hit.collider.transform.GetComponent<brain>()) != null) hiveMind.updateTarget(tmpBrain);
                else hiveMind.updateTarget(null);
            } else {
                hiveMind.updateTarget(null);
            }
        }

        /// <summery> a basic is grounded check </summery>
        public static bool isGrounded(this playerController pc, float multiplier = 1.1f, float distance = 0f) {
            if (Physics.Raycast(pc.transform.position, -Vector2.up, out RaycastHit hit, distance == 0f ? Vector3.Distance(pc.transform.position, pc.groundCheck.position) * multiplier : distance)) {
                if (hit.collider.gameObject.layer == sys.var.layers.ground || hit.collider.gameObject.layer == sys.var.layers.ingoreRPGround) {

                    if (hit.collider.gameObject.transform.GetComponent<damageOnHit>() == null) {
                        pc.lastSafePos = new Vector3(hit.collider.bounds.center.x, hit.point.y, hit.collider.bounds.center.z);
                    }

                    return true;
                }
                return false;
            } else {
                return false;
            }
        }

        /// <summery> attack utility </summery>
        /// allows attack animations to run an attack on the player which isnt included in the basic attack,
        ///     This can be used for attacks which attack with an animation key
        public static void extraAttack(this playerController pc) {
            pc.attack.extraAttack(pc);
        }
    }
}