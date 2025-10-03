using UnityEngine;

public class EN_base : MonoBehaviour {
    [Header("componenets")]
    public brain brain;
    public Rigidbody rb;
    public GameObject player;

    [Header("heath")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("sounds")]
    public AudioClip hurtsound;
    public AudioClip deathSound;

    [Header("data")]
    public string playerTag = "Player";
    [Range(0f, 100f)] public float moveSpeed;

    void Start() {
        brain = GetComponent<brain>();
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectsWithTag(playerTag)[0];
    
        brain.thought = $"{currentHealth}/{maxHealth}";
    }

    void Update() {
        // movement
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * moveSpeed);
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
        Destroy(transform.gameObject);
        AudioSource.PlayClipAtPoint(deathSound, transform.position);
    }
}
