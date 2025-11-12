using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// [RequireComponent(typeof(Rigidbody))]
public class playerController : MonoBehaviour {
    /// <summery> variables </summery>
    #region variables
        
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
    #endregion

    /// <summery> basic start </summery>
    #region Start
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
    #endregion

    /// <summery> basic update </summery>
    #region Update
        void Update() {
            // attack update
            if (attack != null) attack.update(this); 

            if (health <= 0) return;
            if (!loaded) return;
            if (GS.live.state.paused || GS.live.state.helped || GS.live.state.menued) return;

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
    #endregion

    /// <summery> utilities to seperate the movement from the update function </summery>
    /// just for clenliness really
    #region movementUtils 
        /// <summery> This is what allows the player to look around and what not </summery>
        void HandleMouse() {
            float mouseX = Input.GetAxis("Mouse X") * RT_Modifier;
            
            // This is so fun and silly (unused)
            float mouseY = 0f;
            if (cameraY) mouseY = Input.GetAxis("Mouse Y") * RT_Modifier;

            mouseX = eevee.input.CheckAxis("cameraRight", "cameraLeft") == 0 ? mouseX : eevee.input.CheckAxis("cameraRight", "cameraLeft") * RT_Modifier;

            transform.Rotate(Vector3.up * mouseX);
            if(cameraY) camera.Rotate((Vector3.right * -mouseY));
        }

        /// <summery> allows the player to add a set amount of velocity to the player </summery>
        /// it also clears the players Y velocity 
        void jump() {
            if (jumpCount <= 0) return; // if cant jump then dont

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce);
            jumpCount--;
            StartCoroutine(allowNextJump());
        }

        /// <summery> This function basically checks if the player can dash and then starts the `dasher` coroutine </summery>
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

    /// <summery> utilities to allow basic things, such as resetting the players animators </summery>
    #region utils
        /// <summery> resets the players animators </summery>
        /// This is something i plan on phasing out as it adds an extra layer of complexity to the attacks which is un-needed as there will never be a point when the player doesnt have an attack loaded
        public void Reset() {
            AttackDisplay.runtimeAnimatorController = D_AttackDisplay;
            crosshairDisplay.runtimeAnimatorController = D_crosshair;
            attack = D_Attack;
        }

        /// <summery> allows the player to chain their main attack <summery>
        /// This function should call the 
        ///     attack.load
        ///     attack.unLoad
        /// functions to allow for things like stat editing when you use a weapon and switching the animators
        public void switchAttack(AT_base newAttack) {
            // Reset(); // should work fine without this
            attack.unLoad(this);

            attack = Instantiate(newAttack);
            
            attack.load(this);

            save.saveData currentSave = save.getData.viewSave();

            // save attack
                currentSave.currentAttack = attack.name;
                currentSave.currentAttackData = attack.attackData;

            save.getData.save(currentSave);
        }

        /// <summery> </summery>
        /// This is a really important function i recommend coming back to
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

        /// <summery> a basic is grounded check </summery>
        public bool isGrounded(float multiplier = 1.1f, float distance = 0f) {
            return Physics.Raycast(transform.position, -Vector2.up, distance == 0f ? Vector3.Distance(transform.position, groundCheck.position) * multiplier : distance);
        }

        /// <summery> attack utility </summery>
        /// allows attack animations to run an attack on the player which isnt included in the basic attack,
        ///     This can be used for attacks which attack with an animation key
        public void extraAttack() {
            attack.extraAttack(this);
        }

    #endregion

    /// <summery> health system, damage, heal, die.. etc </summery>
    #region health
        /// <summery> DealDamage </summery>
        /// this would typically apply a single point of damage unless i wanted to do a silksong and be horribly evil
        public void DealDamage(int damage = 1, Transform dealer = null, bool nockback = true) {
            damage = loaded ? damage : 0;
            
            health -= damage;
            health = Math.Clamp(health, 0, maxHealth);

            if (health <= 0) Die();
            else {
                AS.clip = hurtsound;
                AS.Play();
                // transform.GetComponent<cameraTilt>().shake(25, 2);
            }

            if (dealer != null && nockback) {
                Vector3 force = sys.nockback.calculateNockback(transform.position, dealer.position, 400f);

                rb.AddForce((dealer.forward * 400) + new Vector3(0, 20, 0));
            }

            healthDisplay.text = $"{health}/{maxHealth}";
            // if (damage > 0) hud.displayText(profanities.Count > 0 ? profanities[UnityEngine.Random.Range(0, profanities.Count - 1)] : "owwwww", Color.red);
        }

        /// <summery> heal </summery>
        /// this would again typically heal a single point of health, but can be overriden by things like life steal
        public void heal(int damage = 1) {
            health += damage;

            health = Math.Clamp(health, 0, maxHealth);
            healthDisplay.text = $"{health}/{maxHealth}";
        }

        /// <summery> die </summery>
        /// This originally just deleted the player but now it does one of two things
        ///     Check if the player has instant respawn turned on
        ///         if yes reload the scene
        ///         if no turn on the death screen and wait for an input
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

    /// <summery> coroutines </summery>
    #region IEunmerators
        /// <summery> This is a util to allow me to wait before running code without making a custom IEnumerator for each <summery>
        public IEnumerator waitForTime(System.Action input, float time) {
            yield return new WaitForSeconds(time);
            input();
        }

        /// <summery> This is a util similar to waitForTime which waits for a specified key to be pressed <summery>
        /// at some point id like to make one for general any input at all
        public IEnumerator waitForInput(System.Action input, string key = "interact") {
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => eevee.input.Grab(key));
            input();
        }

        /// <summery> every frame while a key is held run some code <summery>
        /// halt : bool
        ///     remove input while held
        /// halfVerScale: bool
        ///     Half the verticle scale of the player while the button is held
        /// This is mostly used for the dash i beleive
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

        /// <summery> moves the player forwards constantly </summery>
        /// This also decays the players velocity by a set amount which is slower than when not sliding
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

        /// <summery> adds force downwards on the player while removing their ability to move </summery>
        /// if the slam button is still pressed after its finished move the player into a slide
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

        /// <summery> similar to slide, yet this freezes the player in air and stops time <summery>
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

        /// <summery> a very basic jump delay </summery>
        /// might be better to make this use a `waitForTime` function to assist in readability while this method makes it easier for me to seperate and keep track of
        public IEnumerator allowNextJump() {
            canResetJump = false;
            yield return new WaitForSeconds(0.1f);
            canResetJump = true;        
        }

    #endregion

    /// <summery> some basic dev functions, like OnDrawGizmos </summery>
    #region dev
        /// <summery> basic onDrawGizmos </summery>
        /// adds a line in the direction the player is facing
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * dashDistance);
        }
    #endregion

}

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