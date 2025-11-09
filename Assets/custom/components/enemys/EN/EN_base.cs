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

    [Header("sounds")]
    public AudioClip spawnSound;
    public AudioClip hurtsound;
    public AudioClip deathSound;

    [Header("data")]
    [Range(0f, 100f)] public float moveSpeed;

    public string playerTag = "Player";
    public AT_base attack;

    [Range(0, 5f)] public float pathCalculationDelay = 2.5f;

    // movement
    private bool canMove = true;
    private UnityEngine.AI.NavMeshAgent NV_Agent; // should be in the components but oh wells


    /*
        Start, initialise pathFinding and such
    */
    void Start() {
        // sounds
        if (spawnSound != null) AudioSource.PlayClipAtPoint(spawnSound, transform.position);

        // grab components
        brain = GetComponent<brain>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        NV_Agent = GetComponent<NavMeshAgent>();

        // load Nav Mesh data
        NV_Agent.speed = moveSpeed;

        // grab player
        player = GameObject.FindGameObjectsWithTag(playerTag)[0];

        // load my attack
        if (attack != null) {
            attack = Instantiate(attack);
            attack.enemyLoad(this);
            sr.sprite = attack.sprite;
        }

        // set health
        dead = false;
        currentHealth = maxHealth;
        brain.thought = $"{currentHealth}/{maxHealth}";

        // start co routines
        StartCoroutine(movement());
    }

    /*
        Update, movement and what not
    */
    void Update() {
        if (dead) return;

        // movement
        if (attack != null) {
            if (Vector3.Distance(transform.position, player.transform.position) > attack.range * 0.9) {
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "idle") anim.Play("walking");
                canMove = true;
            } else {
                attack.EN_attack(this);
                canMove = false;
            }
        } else {
            canMove = true;
        }
    }

    #region utils
    // DealDamage
    public bool DealDamage(int damage, Transform dealer = null, bool nockback = true, float nockbackForce =  400f) {
        if (dead) return false; // idk why i didnt do this originally
        bool killed = false;

        anim.Play("hurt");
        if (currentHealth > 0) currentHealth -= damage;
        brain.thought = $"{currentHealth}/{maxHealth}";

        if (currentHealth <= 0) {
            Die();
            killed = true;
        }
        
        else AudioSource.PlayClipAtPoint(hurtsound, transform.position);

        if (dealer != null && nockback) {
            rb.AddForce((dealer.forward * nockbackForce) + new Vector3(0, 20, 0));
        }

        return killed;
    }

    // die
    public void Die() {
        dead = true;
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        if(!sr) Destroy(sr.transform.gameObject);
        NV_Agent.enabled = false;
        anim.Play("die");
    }
    #endregion

    /*
        CoRoutine to keep track of the player and if the player can move
    */
    public IEnumerator movement() {
        while (true && !dead && NV_Agent.enabled) {
            NV_Agent.SetDestination(canMove ? player.transform.position : transform.position);
            
            yield return new WaitForSeconds(pathCalculationDelay);

            float passedTime = 0f;
            bool cached = canMove;
            while (passedTime < pathCalculationDelay && cached == canMove) passedTime += Time.deltaTime;
        }
    }
}
