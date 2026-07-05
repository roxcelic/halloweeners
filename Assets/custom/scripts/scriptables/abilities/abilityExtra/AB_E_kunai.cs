using UnityEngine;

public class AB_E_kunai : damageOnHit {
    public bool onAnyHit = false;

    public override void onEnemyHit(Transform hit) {
        AB_kunai.hit = hit;
        Debug.Log("hi");
    }

    public override void onHit(Transform hit) {
        if (!onAnyHit) return;

        AB_kunai.hit = hit;
        Debug.Log("hi");
    }
}