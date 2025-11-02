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

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/enemys/ricardo")]
public class AT_enemySpecific : AT_base {
    [Header("enemyData")]
    public string attackAnimationName = "attack";

    /*

    */
    public override AT_base protection(){
        AT_enemySpecific newInstace = ScriptableObject.CreateInstance<AT_enemySpecific>();

        var type = typeof(AT_enemySpecific);
        
        foreach (var sourceProperty in type.GetProperties()) {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty.SetValue(newInstace, sourceProperty.GetValue(this, null), null);
        }

        foreach (var sourceField in type.GetFields()) {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(newInstace, sourceField.GetValue(this));
        }   

        Debug.Log("ricardo is loading his attack");

        return newInstace;   
    }

    public override void attack(playerController character) {Debug.Log("how did you get this");}

    public override void EN_attack(EN_base enemy) {
        if (canShoot) enemy.anim.Play(attackAnimationName);
        base.EN_attack(enemy);
    }
}
