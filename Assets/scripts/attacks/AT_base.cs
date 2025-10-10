using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/* 
    Animations needed:
        - attackDisplay
            - idle
            - attack
        - crosshair
            - idle
        - enemy display
            - idle
            - attack
*/

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/base")]
public class AT_base : ScriptableObject {
    [Header("basic values")]
    [Range(0f, 25f)] public float range = 25f;
    [Range(0f, 25f)] public float shootDelay = 1f;
    [Range(0f, 25f)] public float enemyShootDelay = 1f;
    public bool canShoot = true;

    [Header("display")]
    public string name = "base";

    public Sprite sprite;

    public RuntimeAnimatorController AC; 
    public RuntimeAnimatorController crosshair;
    public RuntimeAnimatorController enemyDisplay;

    [Header("sounds")]
    public AudioClip SF_fire;

    [Header("ammo")]
    public bool useAmmo = true;
    public int useageAmmo = 1;
    public int maxAmmo = 6;
    public int currentAmmo = 6;

    [Header("damage")]
    [Range(0f, 25f)] public float damage = 10f;

    // do not change
    public virtual AT_base protection(){
        AT_base newInstace = ScriptableObject.CreateInstance<AT_base>();

        var type = typeof(AT_base);
        
        foreach (var sourceProperty in type.GetProperties()) {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty.SetValue(newInstace, sourceProperty.GetValue(this, null), null);
        }

        foreach (var sourceField in type.GetFields()) {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(newInstace, sourceField.GetValue(this));
        }   

        return newInstace;   
        // return this;
    }

    /*
        This method is called when first loading the attack into the player
        you can use this to load default values or add animations like i am doing
    */
    public virtual void load(movement character) {
        // animators
        character.AttackDisplay.runtimeAnimatorController = AC;
        character.crosshairDisplay.runtimeAnimatorController = crosshair;

        // data
        canShoot = true;
        currentAmmo = maxAmmo;
    }

    /* 
        same as above but for enemys, no unload because an unload is death
    */
    public virtual void enemyLoad(EN_base enemy) {
        // animators
        enemy.AttackDisplay.runtimeAnimatorController = enemyDisplay;

        // data
        canShoot = true;
        currentAmmo = maxAmmo;
    }

    /*
        This method is called when the attack is taken off the player
        use this to un do whatever load does typically
    */
    public virtual void unLoad(movement character) {
        character.Reset();
    }

    /*
        Basic attack, you can use this for whatever really
    */
    public virtual void attack(movement character) {
        if (!canShoot) return; // if the attack cannot be used return
        if (useAmmo && (currentAmmo - useageAmmo) < 0) return; // if the attack uses ammo and the user has no ammo, return

        // effects
        character.Shot_effect.Play("flash");
        character.AttackDisplay.Play("attack");
        
        // attack itself
        Collider hit = runHit(character.transform);
    
        // do something with the attack
        if (hit) {
            EN_base enemey = null;

            character.AS.clip = SF_fire;
            character.AS.Play();

            if ((enemey = hit.transform.GetComponent<EN_base>()) != null) {
                // sound
                enemey.DealDamage((int)damage, character.transform);
            }
        }

        // shoot delay
        character.StartCoroutine(fireCondition(shootDelay));

        // lower ammo
        if (useAmmo) currentAmmo -= useageAmmo;
    }

    /*
        Basic attack but for an enemy
    */
    public virtual void EN_attack(EN_base enemy) {
        if (!canShoot) return; // if the attack cannot be used return
        if (useAmmo && (currentAmmo - useageAmmo) < 0) return; // if the attack uses ammo and the user has no ammo, return

        // effects
        enemy.AttackDisplay.Play("enemyAttack");
        
        // attack itself
        Collider hit = runHit(enemy.transform);
    
        // do something with the attack
        if (hit) {
            movement player = null;

            AudioSource.PlayClipAtPoint(SF_fire, enemy.transform.position);

            if ((player = hit.transform.GetComponent<movement>()) != null) {
                // sound
                player.DealDamage();
            }
        }

        // shoot delay
        enemy.StartCoroutine(fireCondition(enemyShootDelay));

        // lower ammo
        if (useAmmo) currentAmmo -= useageAmmo;
    }

    /*
        I use this to define the condition i want the player to fire under, so a simple delay will do for the basic attack
    */
    public virtual IEnumerator fireCondition(float delay) {
        Debug.Log("waiting fire condition");
        canShoot = false;
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    /*
        This is what ill use to get a target, so i can run this multiple times with multiple offsets for something like a shotgun
    */
    public Collider runHit(Transform character, float offset = 0) {
        RaycastHit hit;
        Vector3 targetDirection = Vector3.forward + new Vector3(offset, 0, 0);

        if (Physics.Raycast(character.position, character.TransformDirection(targetDirection), out hit, range)) { 
            Debug.DrawRay(character.position, character.TransformDirection(targetDirection) * hit.distance, Color.red, 2); 
        
            Debug.Log($"hit target: {hit.collider.gameObject.name} on layer {hit.collider.gameObject.layer}"); // debug

            return hit.collider;
        }

        return null;
    }

    /*
        reload, i have no plans to add a way to reload into the actual game, meaning that is should be called with a special ability of some kind
    */
    public void reload() {
        currentAmmo = maxAmmo;
    }
}
