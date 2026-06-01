using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

using save;

using player.utils;
using player.move;
using player.health;
using player.abil;
using player.abil.dash;

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

            public Vector3 lastSafePos;

            [Header("rotation")]
            public bool canMoveCamera = true;
            public bool cameraY = false;
            public float cameraClamp = 40f;
            [Range(0f, 15f)] public float RT_Modifier = 5f;
            new public Transform camera;

        
        [Header("Jump")]
        [Range(0, 1000f)] public float jumpForce;
        public bool canResetJump = true;
        public bool canJump = true;
        public int jumpCount = 1;
        public int maxJumpCount = 1;

        [Header("slope handeling")]
        public float maxSlopAngle = 40f;
        public string rampTag = "ramp";
        public RaycastHit slopeHit;

        [Header("slide")]
        public bool sliding = false;
        public float crouchDistance = 0.5f;
        public float slideDecay;
        public float stopSpeed;
        public float maxForceForSlideAddition = 40;

        [Header("dash")]
        public bool canDash = true;
        public bool isDashing = false;
        [Range(0, 25f)] public float dashDistance, outDashForce, dashSpeed;
        public float dashDelay = 1f;

        [Header("componenets")]
        // pause menu
        public pauseMenuController pauseMenu;

        // this is basic unity stuff
        public Rigidbody rb;
        public Collider col;
        public AudioSource AS;

        // groundCheck
        public Transform groundCheck;

        // attack information
        public Animator ScreenEffect, AttackDisplay, abilityCharge;

        public GameObject deathScreen, UI_HUD, UI_dev, UI_cover, UI_stats;

        public TMP_Text thoughtDisplay;

        // info display
        public hudDisplay hud;

        [Header("data -- custom")]
        public AT_base attack;
        public AB_base ability;
        [Range(1f, 15f)] public float range = 5f;

        [Header("defaults")]
        public RuntimeAnimatorController D_AttackDisplay;

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

        /// <summery> no clip </summery>
        public bool noclip {
            get {return M_noclip;}
            set {
                if(rb == null || col == null) return;

                rb.useGravity = !value;
                col.isTrigger = value;
                M_noclip = value;
            }
        }

        private bool M_noclip = false;

        /// <summery> something to restrict all movement but also say why </summery>
        public enum movementRestriction {
            none,
            dashBlock
        }
        public movementRestriction restrictions = movementRestriction.none;

        /// <summery> a little variable to store the velocity during stop and start </summery>
        private Vector3 metaControllerVelocity = new Vector3();

        // tracks if the player has moved or not
        private bool moved = false;
    #endregion

    /// <summery> basic start </summery>
    #region Start
        protected virtual void Start() {
            // set the player refrence globally
            mainPlayer = this;
            
            // get the components
            rb = GetComponent<Rigidbody>();
            AS = GetComponent<AudioSource>();

            // velocity
            addVel = new movement.additionalVelocity(0, updateSpeed, true);
            StartCoroutine(addVel.start(rb));

            // curser
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // health
            health = maxHealth;
            this.DealDamage(0);

            // get the saved data
            save.saveData currentSave = save.getData.viewSave();
            AT_base savedAttack = GS.live.state.getCurrentAttack(currentSave.currentAttack);
            AB_base savedAbility = GS.live.state.getCurrentAbility(savedAttack.ability);

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
            // absolute movement restriction
            if(restrictions != movementRestriction.none) return;

            // noclip
            if (noclip) {
                this.HandleMouse();
                 if (eevee.input.Collect("Attack", "PC") && attack != null) attack.attack(this);

                transform.noClip(camera);
                rb.linearVelocity = new Vector3();

                return;
            }

            // attack update
            if (attack != null) attack.update(this); 
            if (ability != null) ability.update(this);

            if (health <= 0) return;
            if (!loaded) {
                rb.linearVelocity = new Vector3();
                return;
            }
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

                        targetVelocity = this.getSlopeModeDirection(targetVelocity) * moveSpeed;
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

                    if (!moved && (
                        eevee.input.Check("up") ||
                        eevee.input.Check("down") ||
                        eevee.input.Check("left") ||
                        eevee.input.Check("right")
                    )) {
                        moved = true;
                        GS.live.state.moved = true;
                    }
                    

                // abilies
                    if (eevee.input.Grab("Jump")) this.jump();
                    if (eevee.input.Grab("Slam")) {
                        if (!this.isGrounded()) StartCoroutine(this.slam());
                        else StartCoroutine(this.slide());
                    }
                    if (eevee.input.Grab("Dash")) this.dash();
            }

            // camera rotation
            if(canMoveCamera) this.HandleMouse();

            if (eevee.input.Collect("Attack", "PC")) {
                this.impThoughts();
                if(attack != null) attack.attack(this);
            }
            if (eevee.input.Grab("interact", "PC")) this.impThoughts();
            if (eevee.input.Grab("Ability", "PC")) {
                StartCoroutine(this.whileHeld(
                    () => {},
                    "Ability",
                    false,
                    false,
                    () => {abilityCharge.Play("Charge");},
                    () => {abilityCharge.Play("idle");}
                ));
            }

            // thoughts
            this.ViewThoughts();

            if (this.isGrounded() && canResetJump && CanMove) {
                jumpCount = maxJumpCount;
            } 

            if (liveIftames > 0) liveIftames--;
        }
    #endregion

    /// <summery> a quicksave for the characters weapon </summery>
    public void quicksave() {
        saveData CS = getData.viewSave();
        if(attack != null) {
            CS.currentAttackData = attack.attackData;
            CS.currentAttack = attack.name;
        } else { 
            CS.currentAttackData = new attack.attackData();
            CS.currentAttack = "";
        }
        getData.save(CS);
    }

    #region play&sotp
    public void play() {
        CanMove = true;
        rb.useGravity = true;
        rb.linearVelocity = metaControllerVelocity;
        col.isTrigger = false;
        restrictions = movementRestriction.none;
    }
    public void stop(movementRestriction cause) {
        CanMove = false;
        rb.useGravity = false;
        metaControllerVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3();
        col.isTrigger = true;
        restrictions = cause;
    }
    #endregion
    #region  hide ui
    public void hideUI(bool state) {
        UI_cover.SetActive(state);
        UI_dev.SetActive(state);
        UI_HUD.SetActive(state);
        UI_stats.SetActive(state);
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