using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/base")]
public class AT_base : ScriptableObject {
    [Header("basic values")]
    [Range(0f, 25f)] public float range = 25f;
    [Range(0f, 25f)] public float shootDelay = 1f;
    public bool canShoot = true;

    [Header("display")]
    public AudioClip[] Sounds;
    public RuntimeAnimatorController AC; 


    public virtual void start(movement character) {
        character.AttackDisplay.runtimeAnimatorController = AC;
    }

    public virtual void attack(movement character) {
        if (!canShoot) return;
        // effects
        character.Shot_effect.Play("flash");
        
        // attack itself
        RaycastHit hit;

        if (Physics.Raycast(character.transform.position, character.transform.TransformDirection(Vector3.forward), out hit, range)) { 
            Debug.DrawRay(character.transform.position, character.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow, 2); 
        
            Debug.Log($"hit target: {hit.collider.gameObject.name}");

            // sound
            character.AS.clip = Sounds[0];
            character.AS.Play();

            character.StartCoroutine(fireCondition());
        }
    }

    public virtual IEnumerator fireCondition() {
        canShoot = false;
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }
}
