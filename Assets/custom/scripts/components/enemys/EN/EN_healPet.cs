using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

using player.health;

public class EN_healPet : EN_base {
    public override bool DealDamage(int damage, Transform dealer = null, bool nockback = true, float nockbackForce = 1f) {
        playerController.mainPlayer.heal(10);

        return base.DealDamage(damage, dealer, nockback, nockbackForce);
    }
}