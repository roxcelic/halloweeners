using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/Grapple")]
public class AT_Grapple : AT_base {
    [Header("grapple settings")]
    public float grappleRange = 100f;

    public float newPlayerMoveSpeed = 50f;
    private float playerMoveSpeed = 0f;

    public float newPlayerDamageModifier = 50f;
    private float playerDamageModifier = 0f;

    // load and what not
    public override void load(playerController character) {
        character.AttackDisplay.runtimeAnimatorController = AC;
        character.crosshairDisplay.runtimeAnimatorController = crosshair;

        playerMoveSpeed = character.moveSpeed;
        character.moveSpeed = newPlayerMoveSpeed;

        playerDamageModifier = character.D_Attack.attackData.damageModifier;
        character.D_Attack.attackData.damageModifier = newPlayerDamageModifier;
    }
    public override void safeLoad(playerController character) {
        character.AttackDisplay.runtimeAnimatorController = AC;
        character.crosshairDisplay.runtimeAnimatorController = crosshair;
        
        playerMoveSpeed = character.moveSpeed;
        character.moveSpeed = newPlayerMoveSpeed;
        
        playerDamageModifier = character.D_Attack.attackData.damageModifier;
        character.D_Attack.attackData.damageModifier = newPlayerDamageModifier;
    }

    public override void unLoad(playerController character) {
        Debug.Log("unloading");
        character.Reset();
        
        character.moveSpeed = playerMoveSpeed;
        character.D_Attack.attackData.damageModifier = playerDamageModifier;
    }


    public override void attack(playerController character) {
        
        RaycastHit hit;
        if (Physics.Raycast(character.transform.position, character.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
            character.StartCoroutine(grapple(character, hit.point));
        }

    }

    public IEnumerator grapple(playerController character, Vector3 target) {
        Time.timeScale = 0f;
        character.CanMove = false;

        while (Vector3.Distance(character.transform.position, target) > 0.1f) {
            character.transform.position = Vector3.Lerp(character.transform.position, target, Time.fixedDeltaTime * 5f);
            yield return 0;
        }
        
        Time.timeScale = 1f;
        character.CanMove = true;
    }
}
