using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/enemys/ricardo")]
public class AT_enemySpecific : AT_base {
    [Header("enemyData")]
    public string attackAnimationName = "attack";

    public override void attack(playerController character) {Debug.Log("how did you get this");}

    public override void EN_attack(EN_base enemy) {
        if (canShoot) enemy.anim.Play(attackAnimationName);

        base.EN_attack(enemy);
    }
}
