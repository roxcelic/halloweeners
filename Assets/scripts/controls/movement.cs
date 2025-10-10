using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class movement : MonoBehaviour {

    [Header("data")]
    [Range(0, .3f)] public float MovementSmoothing = .05f;
    public float moveSpeed = 5f;
	
    public Vector3 Velocity = Vector3.zero;

    public float currentXRotation;

    // movement bools
    public bool SmoothMovement = true;
    public bool Control = false;
    public bool CanMove = true;

    [Header("rotation")]
    [Range(0f, 15f)] public float RT_Modifier = 5f;
    public Vector3 camera;

    [Header("componenets")]
    public Rigidbody rb;
    public Collider col;
    public AudioSource AS;

    public Animator Shot_effect;
    public Animator AttackDisplay;
    public Animator crosshairDisplay;

    public GameObject playerCamera;
    public GameObject deathScreen;

    public TMP_Text thoughtDisplay;
    public TMP_Text healthDisplay;

    [Header("data -- custom")]
    public AT_base attack;

    [Header("defaults")]
    public AT_base D_Attack;
    public RuntimeAnimatorController D_AttackDisplay;
    public RuntimeAnimatorController D_crosshair;

    public List<Pickup> interactables;

    [Header("stats")]
    public int maxHealth;
    public int health;

    [Header("sounds")]
    public AudioClip hurtsound;
    public AudioClip deathSound;


    void Start() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        AS = GetComponent<AudioSource>();

        // curser
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // health
        health = maxHealth;
        DealDamage(0);

        // load attack
        attack = attack.protection();
        attack.load(this);
    }

    void Update() {

        #region movement
        if (CanMove) {

            // get the desired force
            Vector3 targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down") * moveSpeed;
            targetVelocity += transform.right * eevee.input.CheckAxis("right", "left") * moveSpeed;

            if (SmoothMovement && Control){ // apply it naturally
                rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref Velocity, MovementSmoothing);
            } else { // apply it forcefully
                rb.AddForce(targetVelocity);
            }

            // camera rotation
            HandleMouse();
        }
        #endregion

        #region attack
        if (eevee.input.Grab("Attack")) {
            attack.attack(this);
        }
        #endregion

        // thoughts
        ViewThoughts();
    }

    // stolen from the amazing A curr, thank you queen.
    void HandleMouse() {
        float mouseX = Input.GetAxis("Mouse X") * RT_Modifier;

        transform.Rotate(Vector3.up * mouseX);

        currentXRotation = Mathf.Clamp(currentXRotation, -90, 90);

        playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
    }

    // reset to default values
    public void Reset() {
        AttackDisplay.runtimeAnimatorController = D_AttackDisplay;
        crosshairDisplay.runtimeAnimatorController = D_crosshair;
        attack = D_Attack;
    }

    // switch attack
    public void switchAttack(AT_base newAttack) {
        Reset();

        attack = newAttack;
        
        attack = attack.protection();
        attack.load(this);
    }

    // thoughts
    public void ViewThoughts() {
        RaycastHit hit;
        brain tmpBrain;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
            if ((tmpBrain = hit.collider.transform.GetComponent<brain>()) != null) thoughtDisplay.text = tmpBrain.thought;
            else thoughtDisplay.text = "";
        } else {
            thoughtDisplay.text = "";
        }
    }

    #region health
    public void DealDamage(int damage = 1, Transform dealer = null, bool nockback = true) {
        health -= damage;

        if (health <= 0) Die();
        else {
            AS.clip = hurtsound;
            AS.Play();
        }

        if (dealer != null && nockback) {
            Debug.Log(dealer);
            Vector3 force = sys.nockback.calculateNockback(transform.position, dealer.position, 400f);
            Debug.Log(force);

            rb.AddForce((dealer.forward * 400) + new Vector3(0, 20, 0));
        }

        string healthText = "";
        for (int i = 0; i < maxHealth; i++) {
            if(i >= health) healthText += "-";
            else healthText += "*";
        }

        healthDisplay.text = $"//health//{healthText}";
    }

    public void Die() {
        deathScreen.SetActive(true);
        StartCoroutine(waitForInput(() => {
            deathScreen.transform.GetComponent<Animator>().Play("deathScreenClose");
        }));
        //Destroy(transform.gameObject);
    }

    #endregion

    #region dev

    #endregion

    #region IEunmerators
    public IEnumerator waitForInput(System.Action input) {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => eevee.input.Grab("interact"));
        input();
    }

    #endregion
}
