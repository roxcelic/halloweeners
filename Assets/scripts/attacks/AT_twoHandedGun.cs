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
    private int shootMode = 0;

    // do not change
    public override AT_base protection(){
        AT_base newInstace = ScriptableObject.CreateInstance<AT_twoHandedGun>();

        var type = typeof(AT_twoHandedGun);
        
        foreach (var sourceProperty in type.GetProperties()) {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty.SetValue(newInstace, sourceProperty.GetValue(this, null), null);
        }

        foreach (var sourceField in type.GetFields()) {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(newInstace, sourceField.GetValue(this));
        }   

        // return newInstace;   
        return this;
    }

    public override void attack(movement character) {
        if (!canShoot) return; // if the attack cannot be used return
        if (useAmmo && currentAmmo <= 0) {
            canShoot = false;
            character.AttackDisplay.Play("reload");
            character.StartCoroutine(waitUntilAnimationIsDone(character, () => {canShoot = true; currentAmmo = maxAmmo;}));

            return;
        } // if the attack uses ammo and the user has no ammo, return

        // effects
        character.Shot_effect.Play("flash");

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
        
        // attack itself
        Collider hit = runHit(character.transform);
    
        // do something with the attack
        AudioSource.PlayClipAtPoint(SF_fire, character.transform.position);

        if (hit) {
            EN_base enemey = null;

            if ((enemey = hit.transform.GetComponent<EN_base>()) != null) {
                // sound
                enemey.DealDamage((int)damage, character.transform);
            }
        }

        // lower ammo
        if (useAmmo) currentAmmo -= useageAmmo;
    }

    public IEnumerator waitUntilAnimationIsDone(movement character, System.Action command = null) {
        yield return 0;
        yield return new WaitUntil(() => character.AttackDisplay.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        if(command != null) command();
    }

    // public void allowShoot() {canShoot = true;}
    // public void resetAmmo() {currentAmmo = maxAmmo;}
}
