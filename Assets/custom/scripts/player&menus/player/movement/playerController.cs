using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

using ext;

// [RequireComponent(typeof(Rigidbody))]
public class playerController : MonoBehaviour {
    /// <summery> variables </summery>
    #region variables
        public static playerController mainPlayer;
        
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

            private Vector3 lastSafePos;

            [Header("rotation")]
            public bool cameraY = false;
            public float cameraClamp = 40f;
            [Range(0f, 15f)] public float RT_Modifier = 5f;
            new public Transform camera;

        
        [Header("Jump")]
        [Range(0, 400f)] public float jumpForce;
        public bool canResetJump = true;
        public bool canJump = true;
        public int jumpCount = 1;
        public int maxJumpCount = 1;

        [Header("slope handeling")]
        public float maxSlopAngle = 40f;
        public string rampTag = "ramp";
        private RaycastHit slopeHit;

        [Header("slide")]
        public float slideDecay;
        public float stopSpeed;
        public float maxForceForSlideAddition = 40;

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
        public Animator ScreenEffect;
        public Animator AttackDisplay;
        public Animator crosshairDisplay;

        // ability
        public Animator abilityCharge;

        public GameObject deathScreen;

        public TMP_Text thoughtDisplay;

        // info display
        public hudDisplay hud;

        [Header("data -- custom")]
        public AT_base attack;
        public AB_base ability;

        [Header("defaults")]
        public AT_base D_Attack;
        public RuntimeAnimatorController D_AttackDisplay;
        public RuntimeAnimatorController D_crosshair;

        [Header("stats")]
        public int maxHealth;
        public int health;
        public int charge;

        [Header("sounds")]
        public AudioClip hurtsound;
        public AudioClip deathSound;

        [Header("extra")]
        public List<sys.Text> profanities;

        [Header("iframes")]
        public int maxIframes = 40;
        public int liveIftames = 100;
    #endregion

    /// <summery> basic start </summery>
    #region Start
        protected virtual void Start() {
            // set the player refrence globally
            mainPlayer = this;
            
            // get the components
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            AS = GetComponent<AudioSource>();

            // velocity
            addVel = new movement.additionalVelocity(0, updateSpeed, true);
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
            AB_base savedAbility = GS.live.state.getCurrentAbility(currentSave.currentAbility);

            if (savedAttack != null) attack = savedAttack;

            if (savedAbility != null) ability = savedAbility;

            // load attack
            attack = Instantiate(attack); // clean perhaps a second time
            attack.load(this);
            attack.attackData = currentSave.currentAttackData; // carry over what it can cause its nice

            // load ability
            ability.start(this);
        }
    #endregion

    /// <summery> basic update </summery>
    #region Update
        protected virtual void Update() {
            // attack update
            if (attack != null) attack.update(this); 
            if (ability != null) ability.update(this);

            if (health <= 0) return;
            if (!loaded) return;
            if (GS.live.state.paused || GS.live.state.helped || GS.live.state.menued) return;

            if (CanMove) {
                // movement
                    // get the desired force
                    Vector3 targetVelocity = new Vector3();

                    // onslope(2)
                    if (false) {
                        targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down");
                        targetVelocity += transform.right * eevee.input.CheckAxis("right", "left");
                        targetVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z) + addVel.getVelocity(this);

                        targetVelocity = getSlopeModeDirection(targetVelocity) * moveSpeed;
                    } else {
                        targetVelocity = transform.forward * eevee.input.CheckAxis("up", "down") * moveSpeed;
                        targetVelocity += transform.right * eevee.input.CheckAxis("right", "left") * moveSpeed;
                        targetVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z) + addVel.getVelocity(this);
                    }

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
            if (eevee.input.Collect("Attack", "PC")) attack.attack(this);
            if (eevee.input.Grab("Ability", "PC")) {
                StartCoroutine(whileHeld(
                    () => {},
                    "Ability",
                    false,
                    false,
                    () => {abilityCharge.Play("Charge");},
                    () => {abilityCharge.Play("idle");}
                ));
            }

            // thoughts
            ViewThoughts();
            if (isGrounded() && canResetJump) {
                jumpCount = maxJumpCount;
            } 

            if (liveIftames > 0) liveIftames--;
        }
    #endregion

    /// <summery> utilities to seperate the movement from the update function </summery>
    /// just for clenliness really
    #region movementUtils 
        /// <summery> This is what allows the player to look around and what not </summery>
        public virtual void HandleMouse() {
            // float mouseX = Input.GetAxis("Mouse X") * RT_Modifier;
            
            // // This is so fun and silly (unused)
            // float mouseY = 0f;
            // if (cameraY) mouseY = Input.GetAxis("Mouse Y") * (RT_Modifier / 2);

            // mouseX = eevee.input.CheckAxis("cameraRight", "cameraLeft") == 0 ? mouseX : eevee.input.CheckAxis("cameraRight", "cameraLeft") * RT_Modifier;

            // transform.Rotate(Vector3.up * mouseX);
            // if(cameraY) {
            //     camera.Rotate((Vector3.right * -mouseY));

            //     camera.localEulerAngles = new Vector3(camera.localEulerAngles.x > 180 ? 360 - Mathf.Clamp(360 - camera.localEulerAngles.x, 0, cameraClamp) : Mathf.Clamp(camera.localEulerAngles.x, 0, cameraClamp), camera.localEulerAngles.y, camera.localEulerAngles.z);
            // }

            float mouseX = Input.GetAxisRaw("Mouse X") * RT_Modifier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * RT_Modifier / 2;

            transform.Rotate(Vector3.up * mouseX);

            currentXRotation -= mouseY;
            currentXRotation = Mathf.Clamp(currentXRotation, -cameraClamp, cameraClamp);

            camera.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

        }

        /// <summery> allows the player to add a set amount of velocity to the player </summery>
        /// it also clears the players Y velocity 
        void jump() {
            if (jumpCount <= 0 || !canJump) return; // if cant jump then dont

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(transform.up * jumpForce);
            jumpCount--;

            StartCoroutine(allowNextJump());
            
            canJump = false;
            StartCoroutine(waitForTime(
                () => {canJump = true;},
                0.25f
            ));
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

        /// <summery> a check to see if the player is on a slop </summery>
        public bool onSlope(float multiplier = 1.1f, float distance = 0f) {
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, distance == 0f ? Vector3.Distance(transform.position, groundCheck.position) * multiplier : distance)) {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

                if (slopeHit.collider.gameObject.tag == rampTag) return false;
                return angle < maxSlopAngle && angle != 0;
            }

            return false;
        }

        /// <summery> find force direction on slope </summery>
        private Vector3 getSlopeModeDirection(Vector3 moveDirection) {
            return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
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

        /// <summery> a util to allow an animation to ran the ability on the player </summery>
        public void runAbility() {
            ability.use(this);
            abilityCharge.Play("idle");
        }

        /// <summery> allows the player to chain their main attack <summery>
        /// This function should call the 
        ///     attack.load
        ///     attack.unLoad
        /// functions to allow for things like stat editing when you use a weapon and switching the animators
        public void switchAttack(AT_base newAttack) {
            // Reset(); // should work fine without this
            if(attack != null) attack.unLoad(this);

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
            if (Physics.Raycast(transform.position, -Vector2.up, out RaycastHit hit, distance == 0f ? Vector3.Distance(transform.position, groundCheck.position) * multiplier : distance)) {
                if (hit.collider.gameObject.layer == sys.var.layers.ground || hit.collider.gameObject.layer == sys.var.layers.ingoreRPGround) {
                    if (hit.collider.gameObject.transform.GetComponent<damageOnHit>() == null) {
                        lastSafePos = new Vector3(hit.collider.bounds.center.x, hit.point.y, hit.collider.bounds.center.z);
                    }

                    return true;
                }
                return false;
            } else {
                return false;
            }
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
        public void DealDamage(int damage = 1, Transform dealer = null, bool nockback = true, float nockbackForce = 1f) {
            if (liveIftames > 0) return;

            damage = loaded ? damage : 0;
            
            health -= damage;
            health = Math.Clamp(health, 0, maxHealth);
            ScreenEffect.Play("hurt");

            if (health <= 0) Die();
            else {
                AS.clip = hurtsound;
                AS.Play();
                // transform.GetComponent<cameraTilt>().shake(25, 2);
            }

            if (dealer != null && nockback) {
                addVel.AddForce(-(nockbackForce));
            }

            liveIftames += maxIframes;

            // if (damage > 0) hud.displayText(profanities.Count > 0 ? profanities[UnityEngine.Random.Range(0, profanities.Count - 1)] : "owwwww", Color.red);
        }

        /// <summery> heal </summery>
        /// this would again typically heal a single point of health, but can be overriden by things like life steal
        public void heal(int damage = 1) {
            health += damage;

            health = Math.Clamp(health, 0, maxHealth);
        }

        /// <summery> die </summery>
        /// This originally just deleted the player but now it does one of two things
        ///     Check if the player has instant respawn turned on
        ///         if yes reload the scene
        ///         if no turn on the death screen and wait for an input
        public void Die() {
            if (save.getData.config().instantRespawn) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else {
                deathScreen.SetActive(true);
                StartCoroutine(waitForInput(() => {
                    deathScreen.transform.GetComponent<Animator>().Play("deathScreenClose");
                }));
            }
        }

        /// <summery> reset pos </summery>
        /// A nice little function to add the glitch effect to the screen and move the player back to where they where last safe
        public void resetToSaftey() {
            ScreenEffect.Play("glitch");
            transform.position = lastSafePos;
            CanMove = false;

            StartCoroutine(waitForTime(() => {
                transform.position = lastSafePos;
                rb.linearVelocity = new Vector3();
                addVel.vel = 0;
                CanMove = true;
            }, 0.1f));
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
        public IEnumerator whileHeld(System.Action input, string key, bool halt = false, bool halfVerScale = false, System.Action before = null, System.Action after = null) {
            if (halfVerScale) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
            if (halt) CanMove = false;
            if (before != null) before();

            while(eevee.input.Check(key)) {
                input();
                yield return 0;
            }
            
            if (halt) CanMove = true;
            if (halfVerScale) transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
            if (after != null) after();
        }

        /// <summery> moves the player forwards constantly </summery>
        /// This also decays the players velocity by a set amount which is slower than when not sliding
        public IEnumerator slide() {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
            
            rb.AddForce(-transform.up * jumpForce * 2f); // thow them down twin

            CanMove = false;
            addVel.update = false;

            Vector3 slideForce = rb.linearVelocity;
            addVel.vel = 0;

            while(eevee.input.Check("Slam") && isGrounded(1, 2f) && (!eevee.input.Check("Jump") || jumpCount <= 0)) {

                rb.linearVelocity = slideForce;

                // x velocity clamp
                if (slideForce.x > stopSpeed) slideForce = new Vector3(slideForce.x - slideDecay, slideForce.y, slideForce.z);
                else if (slideForce.x < -stopSpeed) slideForce = new Vector3(slideForce.x + slideDecay, slideForce.y, slideForce.z);
                else slideForce = new Vector3(0, slideForce.y, slideForce.z);

                // y velocity clamp
                // if (slideForce.y > stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y - slideDecay, slideForce.z);
                // else if (slideForce.y < 0) slideForce = new Vector3(slideForce.x, slideForce.y + slideDecay, slideForce.z);
                // else slideForce = new Vector3(slideForce.x, 0, slideForce.z);


                // z velocity clamp
                if (slideForce.z > stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y, slideForce.z - slideDecay);
                else if (slideForce.z < -stopSpeed) slideForce = new Vector3(slideForce.x, slideForce.y, slideForce.z + slideDecay);
                else slideForce = new Vector3(slideForce.x, slideForce.y, 0);

                slideForce = new Vector3(slideForce.x, 0, slideForce.z);

                yield return 0;
            }

            rb.linearVelocity = new Vector3();

            addVel.update = true;
            addVel.vel = slideForce.magnitude / 2;
            CanMove = true;

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);

            if (eevee.input.Check("Jump") && jumpCount > 0) {
                addVel.AddForce(5);
                jump();
            }
        }

        /// <summery> adds force downwards on the player while removing their ability to move </summery>
        /// if the slam button is still pressed after its finished move the player into a slide
        public IEnumerator slam() {
            CanMove = false;

            Vector3 hldVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3();

            float outForce = 0f;
            while(!isGrounded() && (!eevee.input.Check("Jump") || jumpCount <= 0)) {
                outForce = rb.linearVelocity.y;
                rb.AddForce(-transform.up * jumpForce * 2f);
                yield return new WaitForSeconds(0.1f);
            }

            if (eevee.input.Check("Jump")) {
                jump();
                CanMove = true;
            } else {
                addVel.AddForce(Mathf.Abs(outForce));

                if (eevee.input.Check("Slam")) {
                    rb.linearVelocity = hldVel;

                    StartCoroutine(slide());
                } else CanMove = true;
            }
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

            Time.timeScale = GS.live.state.gameSpeed;
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
        public bool player = false;

        public additionalVelocity(float startVel = 0, float startSpeed = 1, bool player = false ) {
            this.vel = startVel;
            this.updateSpeed = startSpeed;
            this.player = player;
        }

        public Vector3 getVelocity(MonoBehaviour Mono, Vector3 dir = new Vector3()) {return (dir == new Vector3() ? Mono.transform.forward * this.vel : dir * this.vel).Clamp(-maxVel, maxVel);}
        public void AddForce(float force) {this.vel += force;}

        public IEnumerator start(Rigidbody rb) {
            while (true) {
                while (this.update) {
                    bool isPlayerMoving = !player || (
                        eevee.input.Check("left") ||
                        eevee.input.Check("right") ||
                        eevee.input.Check("down") ||
                        eevee.input.Check("up")
                    );

                    float mod = this.updateSpeed * (isPlayerMoving ? 1 : 10);

                    this.vel = Mathf.Lerp(vel, 0, Time.deltaTime * mod);

                    yield return 0;
                }

                yield return new WaitUntil(() => this.update);
            }
        }
    }
}