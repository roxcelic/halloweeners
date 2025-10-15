using UnityEngine;

public class damageOnHit : MonoBehaviour {
    public bool destroyOnHit = true;
    public bool destroyOnContact = false;

    public attackType type = attackType.both;
    [Range(0f, 25f)] public float damage = 10f;

    public enum attackType {
        both,
        player,
        enemy
    }
    
    void OnCollisionEnter(Collision collision) {
        // player
        if (type == attackType.both || type == attackType.player) {
            EN_base enemy = null;
            if ((enemy = collision.transform.GetComponent<EN_base>()) != null) {
                enemy.DealDamage((int)damage, transform);

                if (destroyOnHit) Destroy(transform.gameObject);
            }
        }

        // enemy
        if (type == attackType.both || type == attackType.enemy) {
            playerController player = null;
            if ((player = collision.transform.GetComponent<playerController>()) != null) {
                player.DealDamage(1, transform);

                if (destroyOnHit) Destroy(transform.gameObject);
            }
        }

        // kill
        if (destroyOnContact) Destroy(transform.gameObject);
    }
}