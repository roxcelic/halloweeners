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
    public GameObject player;

    [Header("heath")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    public bool dead = false;

    public movement.additionalVelocity addVel;

    [Header("sounds")]
    public AudioClip spawnSound;
    public AudioClip hurtsound;
    public AudioClip deathSound;

    [Header("data")]

    public string playerTag = "Player";
    public AT_base attack;

    private NEN_base movement;


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
        StartCoroutine(addVel.start(rb));

        // grab player
        player = GameObject.FindGameObjectsWithTag(playerTag)[0];

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
        rb.AddForce(addVel.getVelocity(this));
        if (dead) return;

        // movement
        if (movement != null) {
            if (attack != null) {
                if (Vector3.Distance(transform.position, player.transform.position) > attack.range * 0.9) {
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
        if (dead) return false; // idk why i didnt do this originally
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
        if(sr != null) Destroy(sr.transform.gameObject);
        if (movement != null && movement.NV_Agent != null) movement.NV_Agent.enabled = false;
        anim.Play("die");
    }
    #endregion
}
