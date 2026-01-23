using UnityEngine;

public class AB_E_kunai : damageOnHit {
    public override void onEnemyHit(Transform hit) {
        AB_kunai.hit = hit;
        Debug.Log("hi");
    }
}