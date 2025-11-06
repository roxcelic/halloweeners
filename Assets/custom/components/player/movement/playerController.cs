using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace movement {
    [System.Serializable]
    public class additionalVelocity {
        public float vel = 0;
        public float maxVel = 25f;
        public float updateSpeed = 1f;
        public bool update = true;

        public additionalVelocity(float startVel = 0, float startSpeed = 1 ) {
            this.vel = startVel;
            this.updateSpeed = startSpeed;
        }

        public Vector3 getVelocity(MonoBehaviour Mono) {return Mono.transform.forward * this.vel;}
        public void AddForce(float force) {this.vel += Mathf.Clamp(force, -this.maxVel, this.maxVel);}

        public IEnumerator start(Rigidbody rb) {
            while (true) {
                while (this.update) {
                    this.vel = Mathf.Lerp(vel, 0, Time.deltaTime * (rb.linearVelocity.magnitude > 10 ? this.updateSpeed : this.updateSpeed * 10));

                    yield return 0;
                }

                yield return new WaitUntil(() => this.update);
            }
        }
    }
}

[RequireComponent(typeof(Rigidbody))]
public class playerController : MonoBehaviour {

    [Header("data")]
    // im not sure what this is
        [Range(0, 10f)] public float MovementSmoothing = .05f;
        public float moveSpeed = 5f;
        
        public Vector3 Velocity = Vector3.zero;
        public movement.additionalVelocity addVel;
        [Range(0, 10f)] public float updateSpeed = 1;

        public float currentXRotation;

        // movement bools
        public bool SmoothMovement = true;
        public bool Control = false;
        public bool CanMove = true;
        public bool loaded = false;

        public string targetLayer = "Ground";

        [Header("rotation")]
        public bool cameraY = false;
        [Range(0f, 15f)] public float RT_Modifier = 5f;
        new public Transform camera;
    
    [Header("Jump")]
    [Range(0, 400f)] public float jumpForce;
    public bool canResetJump = true;
    public int jumpCount = 1;
    public int maxJumpCount = 1;

    [Header("slide")]
    public float slideDecay;
    public float stopSpeed;

    [Header("dash")]
    public bool canDash = true;
    [Range(0, 25f)] public float dashDistance = 5f;
    [Range(0, 25f)] public float outDashForce = 5f;
    [Range(0, 25f)] public float dashSpeed = 5f;
    public float dashDelay = 1f;

    [Header("componenets")]
    // this is basic unity stuff
    public Rigidbody rb;
    public Collider col;
    public AudioSource AS;

    // groundCheck
    public Transform groundCheck;

    // attack information
    public Animator Shot_effect;
    public Animator AttackDisplay;
    public Animator crosshairDisplay;

    public GameObject deathScreen;

    public TMP_Text thoughtDisplay;
    public TMP_Text healthDisplay;

    // info display
    public hudDisplay hud;

    [Header("data -- custom")]
    public AT_base attack;

    [Header("defaults")]
    public AT_base D_Attack;
    public RuntimeAnimatorController D_AttackDisplay;
    public RuntimeAnimatorController D_crosshair;

    [Header("stats")]
    public int maxHealth;
    public int health;

    [Header("sounds")]
    public AudioClip hurtsound;
    public AudioClip deathSound;

    [Header("extra")]
    public List<sys.Text> profanities;



    void Start() {
        // get the components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        AS = GetComponent<AudioSource>();

        // velocity
        addVel = new movement.additionalVelocity(0, updateSpeed);
        StartCoroutine(addVel.start(rb));

        // curser
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // health
        health = maxHealth;
        DealDamage(0);

        // get the saved data
        save.saveData currentSave = save.getData.viewSave();
        AT_base savedAttack = GS.live.state.getCurrentAttack(currentSave.currentAttack);

        if (savedAttack != null) {
            Debug.Log($"attack found and loaded: {currentSave.currentAttack}");
            attack = savedAttack;
        }

        // load attack
        attack = Instantiate(attack); // clean perhaps a second time
        attack.load(this);
        attack.attackData = currentSave.currentAttackData; // carry over what it can cause its nice
    }

    void OnDestroy() {
        // save data
        save.saveData currentSave = save.getData.viewSave();

        // save attack
            currentSave.currentAttack = attack.name;
            currentSave.currentAttackData = attack.attackData;

        save.getData.save(currentSave);
    }

    void Update() {
        if (health <= 0) return;
        if (!loaded) return;
        if (GS.live.state.paused || GS.live.state.helped) return;

        if (CanMove) {
            // movement
                // get the desired force
                Vector3 targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down") * moveSpeed;
                targetVelocity += transform.right * eevee.input.CheckAxis("right", "left") * moveSpeed;
                targetVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z) + addVel.getVelocity(this);

                if (SmoothMovement && Control){ // apply it naturally
                    rb.linearVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref Velocity, MovementSmoothing);
                } else { // apply it forcefully
                    rb.AddForce(targetVelocity);
                }
                

            // abilies
                if (eevee.input.Grab("Jump")) jump();
                if (eevee.input.Grab("Slam")) {
                    if (!isGrounded()) StartCoroutine(slam());
                    else StartCoroutine(slide());
                }
                if (eevee.input.Grab("Dash")) dash();
        }

        // camera rotation
            HandleMouse();
            if (eevee.input.Grab("Attack")) attack.attack(this);
            if (eevee.input.Grab("AttackBasic")) {
                D_Attack.safeLoad(this);
                D_Attack.attack(this);
                attack.canShoot = false;
                StartCoroutine(waitForTime(() => {
                    attack.safeLoad(this);
                    attack.canShoot = true;
                }, D_Attack.shootDelay));
            }

            // thoughts
            ViewThoughts();
            if (isGrounded() && canResetJump) {
                jumpCount = maxJumpCount;
            } 
    }

    #region movementUtils 
        // stolen from the amazing A curr, thank you queen.
        void HandleMouse() {
            float mouseX = Input.GetAxis("Mouse X") * RT_Modifier;
            
            // idk i dont plan on using this but why not
            float mouseY = 0f;
            if (cameraY) mouseY = Input.GetAxis("Mouse Y") * RT_Modifier;

            mouseX = eevee.input.CheckAxis("cameraRight", "cameraLeft") == 0 ? mouseX : eevee.input.CheckAxis("cameraRight", "cameraLeft") * RT_Modifier;

            transform.Rotate(Vector3.up * mouseX);
            if(cameraY) camera.Rotate((Vector3.right * -mouseY));
        }

        // a simple jump really
        void jump() {
            if (jumpCount <= 0) return; // if cant jump then dont
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce);
            jumpCount--;
            StartCoroutine(allowNextJump());
        }

        // worlds best dash i think
        void dash() {
            if (!canDash) return;
            
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), dashDistance);
            Vector3 dashForce = new Vector3();

            if (hits.Length > 0) {
                foreach (RaycastHit hit in hits) {
                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer(targetLayer)) {
                        dashForce = transform.forward * (hit.distance - 0.5f);
                        break;
                    }
                }
            } else {
                dashForce = transform.forward * dashDistance;
            }

            StartCoroutine(dasher(transform.position + new Vector3(dashForce.x, 0, dashForce.z)));
        }

    #endregion


    #region utils
        // reset to default values
        public void Reset() {
            AttackDisplay.runtimeAnimatorController = D_AttackDisplay;
            crosshairDisplay.runtimeAnimatorController = D_crosshair;
            attack = D_Attack;
        }

        // switch attack
        public void switchAttack(AT_base newAttack) {
            Reset();

            Debug.Log($"switching to attack with a kill count of {newAttack.attackData.killCount}");

            attack = Instantiate(newAttack);
            
            attack.load(this);

            save.saveData currentSave = save.getData.viewSave();

            // save attack
                currentSave.currentAttack = attack.name;
                currentSave.currentAttackData = attack.attackData;

            save.getData.save(currentSave);
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

        // is grounded
        public bool isGrounded(float multiplier = 1.1f, float distance = 0f) {
            return Physics.Raycast(transform.position, -Vector2.up, distance == 0f ? Vector3.Distance(transform.position, groundCheck.position) * multiplier : distance);
        }

        // a standard attack util for like bleh bleh bleh, currently its being made for attacks during an animation
        public void extraAttack() {
            attack.extraAttack(this);
        }

    #endregion

    #region health
        public void DealDamage(int damage = 1, Transform dealer = null, bool nockback = true) {
            damage = loaded ? damage : 0;
            
            health -= damage;
            health = Math.Clamp(health, 0, maxHealth);

            if (health <= 0) Die();
            else {
                AS.clip = hurtsound;
                AS.Play();
            }

            if (dealer != null && nockback) {
                Vector3 force = sys.nockback.calculateNockback(transform.position, dealer.position, 400f);

                rb.AddForce((dealer.forward * 400) + new Vector3(0, 20, 0));
            }

            healthDisplay.text = $"{health}/{maxHealth}";
            // if (damage > 0) hud.displayText(profanities.Count > 0 ? profanities[UnityEngine.Random.Range(0, profanities.Count - 1)] : "owwwww", Color.red);
        }

        public void heal(int damage = 1) {
            health += damage;

            health = Math.Clamp(health, 0, maxHealth);
            healthDisplay.text = $"{health}/{maxHealth}";
        }

        public void Die() {
            if (save.getData.viewSave().instantRespawn) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else {
                deathScreen.SetActive(true);
                StartCoroutine(waitForInput(() => {
                    deathScreen.transform.GetComponent<Animator>().Play("deathScreenClose");
                }));
            }
        }
    #endregion

    #region IEunmerators
    // wait for time
    public IEnumerator waitForTime(System.Action input, float time) {
        yield return new WaitForSeconds(time);
        input();
    }

    // wait for the player to interact
    public IEnumerator waitForInput(System.Action input, string key = "interact") {
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => eevee.input.Grab(key));
        input();
    }

    public IEnumerator whileHeld(System.Action input, string key, bool halt = false, bool halfVerScale = false) {
        if (halfVerScale) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        if (halt) CanMove = false;

        while(eevee.input.Check(key)) {
            input();
            yield return 0;
        }
        
        if (halt) CanMove = true;
        if (halfVerScale) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    // slide stuff
    public IEnumerator slide() {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
        
        rb.AddForce(-transform.up * jumpForce * 2f); // thow them down twin

        CanMove = false;
        addVel.update = false;

        Vector3 slideForce = rb.linearVelocity + (transform.forward * 5);

        while(eevee.input.Check("Slam") && isGrounded(1, 2f) && !eevee.input.Check("Jump")) {

            rb.linearVelocity = slideForce;

            // x velocity clamp
            if (slideForce.x > stopSpeed) slideForce = new Vector3(slideForce.x - slideDecay, slideForce.y, slideForce.z);
            else if (slideForce.x < -stopSpeed) slideForce = new Vector3(slideForce.x + slideDecay, slideForce.y, slideForce.z);
            else slideForce = new Vector3(0, slideForce.y, slideForce.z);

            if (addVel.vel > stopSpeed) addVel.vel -= slideDecay / 2;
            else if (addVel.vel < -stopSpeed) addVel.vel += slideDecay / 2;
            else addVel.vel = 0;

            // y velocity clamp
            // if (slideForce.y > stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y - slideDecay, slideForce.z);
            // else if (slideForce.y < 0) slideForce = new Vector3(slideForce.x, slideForce.y + slideDecay, slideForce.z);
            // else slideForce = new Vector3(slideForce.x, 0, slideForce.z);


            // z velocity clamp
            if (slideForce.z > stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y, slideForce.z - slideDecay);
            else if (slideForce.z < -stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y, slideForce.z + slideDecay);
            else slideForce = new Vector3(slideForce.x, slideForce.y, 0);

            slideForce = new Vector3(slideForce.x, 0, slideForce.z);

            yield return 0.1f;
        }
        
        addVel.update = true;
        CanMove = true;

        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    
        if (eevee.input.Grab("Jump")) {
            addVel.AddForce(5);
            jump();
        }
    }

    // slam slam slam
    public IEnumerator slam() {
        CanMove = false;

        Vector3 hldVel = rb.linearVelocity;
        Debug.Log("reseting velocity");
        rb.linearVelocity = new Vector3();

        float outForce = 0f;
        while(!isGrounded()) {
            outForce += 1f;
            rb.AddForce(-transform.up * jumpForce * 2f);
            yield return new WaitForSeconds(0.1f);
        }

        if (eevee.input.Check("Slam")) {
            rb.linearVelocity = hldVel;
            addVel.AddForce(outForce);

            StartCoroutine(slide());
        } else CanMove = true;
    }

    // dash dash dash
    public IEnumerator dasher(Vector3 target) {
        Time.timeScale = 0f;
        CanMove = false;
        canDash = false;

        while (Vector3.Distance(transform.position, target) > 1f) {
            Vector3 difference = target - transform.position;
            difference = new Vector3(difference.x, 0, difference.z) * 0.5f;

            transform.position = Vector3.Lerp(transform.position, target + difference, Time.fixedDeltaTime * dashSpeed);
            yield return 0;
        }

        Time.timeScale = 1f;
        CanMove = true;

        addVel.AddForce(outDashForce);

        yield return new WaitForSeconds(dashDelay);
        canDash = true;
    }

    public IEnumerator allowNextJump() {
        canResetJump = false;
        yield return new WaitForSeconds(0.1f);
        canResetJump = true;        
    }

    #endregion



    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * dashDistance);
    }
}
