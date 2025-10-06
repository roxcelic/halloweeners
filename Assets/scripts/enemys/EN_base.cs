using UnityEngine;

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
    public string playerTag = "Player";
    [Range(0f, 100f)] public float moveSpeed;
    public AT_base attack;

    void Start() {
        if (spawnSound != null) AudioSource.PlayClipAtPoint(spawnSound, transform.position);

        brain = GetComponent<brain>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectsWithTag(playerTag)[0];
    
        brain.thought = $"{currentHealth}/{maxHealth}";

        if (attack != null) {
            attack = attack.protection();
            attack.enemyLoad(this);
            sr.sprite = attack.sprite;
        }
    }

    void Update() {
        if (dead) return;
        // movement
        if (attack != null) {
            if (Vector3.Distance(transform.position, player.transform.position) > attack.range * 0.9) {
                anim.Play("walking");
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * moveSpeed);
            } else {
                anim.Play("idle");
                attack.EN_attack(this);
            }
        } else {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * moveSpeed);
        }
    }

    public void DealDamage(int damage, Transform dealer = null, bool nockback = true) {
        currentHealth -= damage;
        brain.thought = $"{currentHealth}/{maxHealth}";

        if (currentHealth <= 0) Die();
        else AudioSource.PlayClipAtPoint(hurtsound, transform.position);

        if (dealer != null && nockback) {
            Debug.Log(dealer);
            Vector3 force = sys.nockback.calculateNockback(transform.position, dealer.position, 400f);
            Debug.Log(force);

            rb.AddForce((dealer.forward * 400) + new Vector3(0, 20, 0));
        }
    }

    public void Die() {
        dead = true;
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
        Destroy(sr.transform.gameObject);
        anim.Play("die");
    }
}
