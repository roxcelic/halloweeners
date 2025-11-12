
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/grapple")]
public class AT_grapple : AT_base {
    [Header("grapple data")]
        public float speed = 2.5f;
        
        public float newPlayerSpeed = 50f;
            private float oldPlayerSpeed;
        public float attackDamage = 100;
            private float oldAttackDamage;
        
        public string targetTag = "enemy";
    
    public override void load(playerController character) {
        oldPlayerSpeed = character.moveSpeed;
        character.moveSpeed = newPlayerSpeed;

        oldAttackDamage = character.D_Attack.damage;
        character.D_Attack.damage = attackDamage;
    }

    public override void attack(playerController character) {
        if (canShoot) {
            Vector3 targetDirection = Vector3.forward;
            RaycastHit[] hits = Physics.RaycastAll(character.transform.position, character.transform.TransformDirection(targetDirection), range);

            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) {
                    if (hit.collider.gameObject.tag == targetTag) {
                        character.StartCoroutine(moveToPoint(hits[0].point, character));

                        break;
                    }
                }
            }

            character.StartCoroutine(fireCondition(shootDelay));
        }
    }

    public override void unLoad(playerController character) {
        character.moveSpeed = oldPlayerSpeed;
        character.D_Attack.damage = oldAttackDamage;
    }

    public IEnumerator moveToPoint(Vector3 target, playerController character){
        Time.timeScale = 0f;
        character.CanMove = false;

        while (Vector3.Distance(character.transform.position, target) > 1f) {
            Vector3 difference = target - character.transform.position;
            difference = new Vector3(difference.x, 0, difference.z) * 0.5f;

            character.transform.position = Vector3.Lerp(character.transform.position, target + difference, Time.fixedDeltaTime * speed);
            yield return 0;
        }

        Time.timeScale = 1f;
        character.CanMove = true;
    }
}
