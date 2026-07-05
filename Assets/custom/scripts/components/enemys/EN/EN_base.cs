using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

public class EN_base : MonoBehaviour {
    [Header("componenets")]
    public brain brain;
    public Rigidbody rb;
    public Animator anim;
    public Animator AttackDisplay;
    public SpriteRenderer sr;

    [Header("heath")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public bool canBeDamaged = true;

    public bool dead = false;

    public movement.additionalVelocity addVel;

    [Header("sounds")]
    public AudioClip spawnSound;
    public AudioClip hurtsound;
    public AudioClip deathSound;

    [Header("data")]
    public bool allowDeath = true;

    public AT_base attack;

    private NEN_base movement;
    [Range(0, 15f)] public float sight = 10f;


    /*
        Start, initialise pathFinding and such
    */
    protected virtual void Start() {
        // sounds
        if (spawnSound != null) AudioSource.PlayClipAtPoint(spawnSound, transform.position);

        // grab components
        brain = GetComponent<brain>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        movement = GetComponent<NEN_base>();

        // vel
        addVel = new movement.additionalVelocity(0, 1);
        if(rb != null) StartCoroutine(addVel.start(rb));

        // load my attack
        if (attack != null) {
            attack = Instantiate(attack);
            attack.enemyLoad(this);
            if (sr != null) sr.sprite = attack.sprite;
        }

        // set health
        dead = false;
        currentHealth = maxHealth;
        brain.thought = $"{currentHealth}/{maxHealth}";

        // start the movmenet
        if (movement != null) movement.begin();
    }

    /*
        Update, movement and what not
    */
    protected virtual void Update() {
        if (dead) return;
        if (rb != null) rb.AddForce(addVel.getVelocity(this));

        // movement
        if (movement != null) {
            if (attack != null) {
                if (Vector3.Distance(transform.position, playerController.mainPlayer.transform.position) > attack.stat.getRange(attack) * 0.9) {
                    if (anim.GetCurrentAnimatorClipInfo(0).Length > 0 && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "idle") anim.Play("walking");
                    movement.canMove = true;
                } else {
                    attack.EN_attack(this);
                    movement.canMove = false;
                }
            } else {
                movement.canMove = true;
            }
        }
    }

    #region utils

    // DealDamage
    public virtual bool DealDamage(int damage, Transform dealer = null, bool nockback = true, float nockbackForce = 1f) {
        if (dead || !canBeDamaged) return false; // idk why i didnt do this originally
        bool killed = false;

        anim.Play("hurt");
        if (currentHealth > 0) currentHealth -= damage;
        brain.thought = $"{currentHealth}/{maxHealth}";

        if (currentHealth <= 0) {
            Die();
            killed = true;
        }
        
        else if (hurtsound != null) AudioSource.PlayClipAtPoint(hurtsound, transform.position);

        if (dealer != null && nockback) {
            addVel.AddForce(-(nockbackForce));
        }

        return killed;
    }

    // die
    public virtual void Die() {
        dead = true;
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position);
        if(allowDeath) Destroy(transform.gameObject);
        if (movement != null && movement.NV_Agent != null) movement.NV_Agent.enabled = false;
        anim.Play("die");

        Instantiate(Resources.Load<GameObject>("effects/explode"), transform.position, Quaternion.identity);
    }

    // revive
    public virtual void Revive() {
        dead = false;
        currentHealth = maxHealth;

        brain.thought = $"{currentHealth}/{maxHealth}";

        if (movement != null && movement.NV_Agent != null) movement.NV_Agent.enabled = true;

        movement.onRevive();
    }
    #endregion
}
