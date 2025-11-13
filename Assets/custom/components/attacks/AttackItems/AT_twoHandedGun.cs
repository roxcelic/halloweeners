using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/* 
    Animations needed:
        - attackDisplay
            - idle
            - attack
            - attack-L
            - attack-R
            - reload
        - crosshair
            - idle
        - enemy display
            - idle
            - attack
*/
[CreateAssetMenu(fileName = "new Attack", menuName = "attacks/two handed gun")]
public class AT_twoHandedGun : AT_base {
    [Header("two handed gun settings")]
    public AudioClip SF_reload;

    private int shootMode = 0;

    public override void attack(playerController character) {
        if (!canShoot) return; // if the attack cannot be used return
        if (useAmmo && currentAmmo <= 0) {
            canShoot = false;
            character.AttackDisplay.Play("reload");
            AudioSource.PlayClipAtPoint(SF_reload, character.transform.position);
            character.StartCoroutine(waitUntilAnimationIsDone(character, () => {canShoot = true; currentAmmo = maxAmmo;}));

            return;
        } // if the attack uses ammo and the user has no ammo, return

        // effects
        character.ScreenEffect.Play("flash");

        if (useAmmo && (currentAmmo - useageAmmo <= 0)) {
            character.AttackDisplay.Play("attack");
            canShoot = false;
            character.StartCoroutine(waitUntilAnimationIsDone(character, () => {canShoot = true;}));
        }
        else if (shootMode == 0) {
            character.AttackDisplay.Play("attack-L");
            canShoot = false;
            character.StartCoroutine(waitUntilAnimationIsDone(character, () => {canShoot = true;}));
            shootMode = 1;
        }
        else if (shootMode == 1) {
            character.AttackDisplay.Play("attack-R");
            canShoot = false;
            character.StartCoroutine(waitUntilAnimationIsDone(character, () => {canShoot = true;}));
            shootMode = 0;
        }

        shoot(character);

        // lower ammo
        if (useAmmo) currentAmmo -= useageAmmo;
        character.hud.displayText($"{currentAmmo}/{maxAmmo}", Color.red);
    }

    public void shoot(playerController character) {
        if (projectile) {
            // effects
            character.ScreenEffect.Play("flash");

            GameObject tmpObj = Instantiate(projectilePrefab, character.transform.position + (character.transform.forward * 2), Quaternion.identity);
            tmpObj.transform.GetComponent<Rigidbody>().AddForce((character.transform.forward * projectileForce) + new Vector3(0, 20, 0));
            tmpObj.transform.GetComponent<damageOnHit>().attributeKill = this;
        
            character.StartCoroutine(fireCondition(shootDelay));
        } else {
            // attack itself
            List<Collider> hits = runHit(character.transform);
        
            AudioSource.PlayClipAtPoint(SF_fire, character.transform.position);

            // do something with the attack
            foreach (Collider hit in hits) {
                EN_base enemey = null;

                if ((enemey = hit.transform.GetComponent<EN_base>()) != null) {
                    // sound
                    if (enemey.DealDamage((int)(damage * attackData.damageModifier), character.transform)) {
                        attackData.killCount++;
                        liveKills++;
                        character.heal((int)(1 * attackData.lifeStealModifer));
                    }
                }
            }
        }
    }

    public IEnumerator waitUntilAnimationIsDone(playerController character, System.Action command = null) {
        yield return 0;
        yield return new WaitUntil(() => character.AttackDisplay.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        if(command != null) command();
    }

    // public void allowShoot() {canShoot = true;}
    // public void resetAmmo() {currentAmmo = maxAmmo;}
}
