using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

using player.health;

public class damageOnHit : MonoBehaviour {
    public bool destroyOnHit = true;
    public bool destroyOnContact = false;
    public bool resetPlayerToSaftey = false;
    public bool parryable = false;
    public playerController attributeKill = null;

    public attackType type = attackType.both;
    [Range(0f, 25f)] public float damage = 10f;
    [Range(0f, 800f)] public float nockbackForce = 400f;

    public enum attackType {
        both,
        player,
        enemy
    }

    [Header("spawn force")]
    public bool spawnForce = false;
    public bool useCharacterDirection = false;
    public Vector3 force;

    [Header("bullet settings")]
    public bool bullet = false;
    public float distance = 50f;
    public Vector3 direction;
    public float speed = 1f;


    void Start() {
        if (bullet) StartCoroutine(shot());
        else {
            if (!spawnForce) return;

            if (useCharacterDirection) force = force.Multiply(playerController.mainPlayer.camera.forward);
            transform.GetComponent<Rigidbody>().AddForce(force);   
        }

    }

    void OnCollisionEnter(Collision collision) {
        // player
        if (type == attackType.both || type == attackType.player) {
            EN_base enemy = null;
            if ((enemy = collision.transform.GetComponent<EN_base>()) != null) {
                if (enemy.DealDamage((int)damage, transform)) if (attributeKill != null) {
                    attributeKill.attack.attackData.killCount++;
                    attributeKill.charge++;
                }

                enemy.rb.AddForce(sys.nockback.calculateNockback(transform.position, enemy.transform.position) * nockbackForce);

                if (destroyOnHit) Destroy(transform.gameObject);
            }
        }

        // enemy
        if (type == attackType.both || type == attackType.enemy) {
            playerController player = null;
            if ((player = collision.transform.GetComponent<playerController>()) != null) {
                player.DealDamage(damage == 0 ? 0 : 1, transform);
                
                if (resetPlayerToSaftey) player.resetToSaftey();
                else player.rb.AddForce(sys.nockback.calculateNockback(transform.position, player.transform.position) * nockbackForce);

                if (destroyOnHit) Destroy(transform.gameObject);

            }
        }

        // kill
        if (destroyOnContact) Destroy(transform.gameObject);
    }

    void OnTriggerEnter(Collider col) {
                // player
        if (type == attackType.both || type == attackType.player) {
            EN_base enemy = null;
            if ((enemy = col.transform.GetComponent<EN_base>()) != null) {
                if (enemy.DealDamage((int)damage, transform)) if (attributeKill != null) {
                    attributeKill.attack.attackData.killCount++;
                    attributeKill.charge++;
                }

                enemy.rb.AddForce(sys.nockback.calculateNockback(transform.position, enemy.transform.position) * nockbackForce);

                if (destroyOnHit) Destroy(transform.gameObject);
            }
        }

        // enemy
        if (type == attackType.both || type == attackType.enemy) {
            playerController player = null;
            if ((player = col.transform.GetComponent<playerController>()) != null) {
                player.DealDamage(damage == 0 ? 0 : 1, transform);
                
                if (resetPlayerToSaftey) player.resetToSaftey();
                else player.rb.AddForce(sys.nockback.calculateNockback(transform.position, player.transform.position) * nockbackForce);

                if (destroyOnHit) Destroy(transform.gameObject);

            }
        }

        // kill
        if (destroyOnContact) Destroy(transform.gameObject);
    }

    public IEnumerator shot() {
        Vector3 startPos = transform.position;

        while(Vector3.Distance(startPos, transform.position) < distance) {
            transform.position = Vector3.Lerp(transform.position, transform.position + direction, Time.deltaTime * speed);
            yield return 0;
        }

        Destroy(transform.gameObject);
    }
}