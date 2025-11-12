using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/base")]
public class AT_base : ScriptableObject {
    /// <summery> variables </summery>
    #region variables
        [Header("basic values")]
        [Range(0f, 100f)] public float range = 25f;
        [Range(0f, 25f)] public float shootDelay = 1f;
        [Range(0f, 25f)] public float enemyShootDelay = 1f;
        public bool canShoot = true;
        public bool lifeSteal = false;
        
        [Header("pierce")]
        public int pierce = 1;
        public bool infinatePierce = false;
        public int wallLayerIndex = 0;

        [Header("projectile")]
        public bool projectile = false;
        public GameObject projectilePrefab;
        public float projectileForce;

        [Header("display")]
        public bool enemyDis = true;
        new public string name = "base";
        public sys.Text displayName = new sys.Text();
        public bool attackWithAnimation = false;

        public Sprite sprite;

        public RuntimeAnimatorController AC; 
        public RuntimeAnimatorController crosshair;
        public RuntimeAnimatorController enemyDisplay;

        [Header("sounds")]
        public AudioClip SF_fire;

        [Header("ammo")]
        public bool useAmmo = true;
        public int useageAmmo = 1;
        public int maxAmmo = 6;
        public int currentAmmo = 6;

        [Header("damage")]
        public float damage = 10f;

        [Header("save data")]
        public attack.attackData attackData = new attack.attackData();
    #endregion

    /// <summery> an update which runs once per frame </summery>
    /// similar to a mono-behaviour
    ///     MonoBehaviour.update => update
    ///     MonoBehaviour.onEnabled => safeLoad
    ///     MonoBehaviour.Start => load
    ///     MonoBehaviour.onDisable => unLoad
    #region update
        public virtual void update(playerController character) {
            // do nothing
        }
    #endregion

    /// <summery> load </summery>
    /// the functions to load data into either the player or enemy
    #region load
        /// <summery> main load </summery>
        /// adds the animatiors to the player character
        public virtual void load(playerController character) {
            // animators
            character.AttackDisplay.runtimeAnimatorController = AC;
            character.crosshairDisplay.runtimeAnimatorController = crosshair;

            // data
            canShoot = true;
            currentAmmo = maxAmmo;
        }

        /// <summery> adds the animators to the player but doesnt reset something like the ammo, preventing free reloads </summery>
        public virtual void safeLoad(playerController character) {
            character.AttackDisplay.runtimeAnimatorController = AC;
            character.crosshairDisplay.runtimeAnimatorController = crosshair;
            canShoot = true;
        }

        /// <summery> loads the animations into the enemy </summery>
        public virtual void enemyLoad(EN_base enemy) {
            // animators
            if (enemyDis) enemy.AttackDisplay.runtimeAnimatorController = enemyDisplay;

            // data
            canShoot = true;
            currentAmmo = maxAmmo;
        }

        /// <summery> this function is used to do something like reset the players stats</summery>
        public virtual void unLoad(playerController character) {
            // do nothing
        }
    #endregion

    /// <summery> attacks </summery>
    #region attack
        /// <summery> a fairly long function, this will run the actual attack code to find what youre aiming at and such </summery>
        public virtual void attack(playerController character) {
            if (!canShoot) return; // if the attack cannot be used return
            if (useAmmo && (currentAmmo - useageAmmo) < 0) return; // if the attack uses ammo and the user has no ammo, return

            if (projectile) {
                // effects
                character.Shot_effect.Play("flash");
                character.AttackDisplay.Play("attack");

                GameObject tmpObj = Instantiate(projectilePrefab, character.transform.position + (character.transform.forward * 2), Quaternion.identity);
                tmpObj.transform.GetComponent<Rigidbody>().AddForce((character.transform.forward * projectileForce) + new Vector3(0, 20, 0));
                tmpObj.transform.GetComponent<damageOnHit>().attributeKill = this;
            
                character.StartCoroutine(fireCondition(shootDelay));
                if (useAmmo) currentAmmo -= useageAmmo;
                character.hud.displayText($"{currentAmmo}/{maxAmmo}", Color.red);
            } else {
                // effects
                character.AttackDisplay.Play("attack");

                // if the attack is run in the animation break here
                if (attackWithAnimation) return;
                
                // attack itself
                List<Collider> hits = runHit(character.transform);
            
                // do something with the attack
                foreach (Collider hit in hits) {
                    EN_base enemey = null;

                    character.AS.clip = SF_fire;
                    character.AS.Play();

                    if ((enemey = hit.transform.GetComponent<EN_base>()) != null) {
                        character.Shot_effect.Play("flash");

                        // sound
                        if (enemey.DealDamage((int)(damage * attackData.damageModifier), character.transform)) {
                            attackData.killCount++;
                            character.heal((int)(1 * attackData.lifeStealModifer));
                        }
                    }
                }

                // shoot delay
                character.StartCoroutine(fireCondition(shootDelay));

                // lower ammo
                if (useAmmo) currentAmmo -= useageAmmo;
                character.hud.displayText($"{currentAmmo}/{maxAmmo}", Color.red);
            }
        }
        
        /// <summery> an "extra attack" this is used to run an attack if the attack is ran via an animation </summery>
        public virtual void extraAttack(playerController character) {
                // pickup where the other attack left off

                // attack itself
                List<Collider> hits = runHit(character.transform);
            
                // do something with the attack
                foreach (Collider hit in hits) {
                    EN_base enemey = null;

                    character.AS.clip = SF_fire;
                    character.AS.Play();

                    if ((enemey = hit.transform.GetComponent<EN_base>()) != null) {
                        // sound
                        if (enemey.DealDamage((int)(damage * attackData.damageModifier), character.transform)) {
                            attackData.killCount++;
                            character.heal((int)(1 * attackData.lifeStealModifer));
                        }

                    }
                }

                // shoot delay
                character.StartCoroutine(fireCondition(shootDelay));

                // lower ammo
                if (useAmmo) currentAmmo -= useageAmmo;
                character.hud.displayText($"{currentAmmo}/{maxAmmo}", Color.red);
        }

        /// <summery> this attack should do the same as the players attack but for any enemy holding a weapon </summery>
        ///     if this weapon is player specific then you dont have to have this
        public virtual void EN_attack(EN_base enemy) {
            if (!canShoot) return; // if the attack cannot be used return
            if (useAmmo && (currentAmmo - useageAmmo) < -1) return; // if the attack uses ammo and the user has no ammo, return

            // effects
            if (enemyDis) enemy.AttackDisplay.Play("enemyAttack");

            if (projectile) {
                GameObject tmpObj = Instantiate(projectilePrefab, enemy.transform.position + (enemy.transform.forward * 2), Quaternion.identity);
                tmpObj.transform.GetComponent<Rigidbody>().AddForce((enemy.transform.forward * projectileForce) + new Vector3(0, 20, 0));
            
                enemy.StartCoroutine(fireCondition(shootDelay));
            } else {
                // attack itself
                List<Collider> hits = runHit(enemy.transform);
            
                // do something with the attack
                foreach (Collider hit in hits) {
                    playerController player = null;

                    AudioSource.PlayClipAtPoint(SF_fire, enemy.transform.position);

                    if ((player = hit.transform.GetComponent<playerController>()) != null) {
                        // sound
                        player.DealDamage();
                    }   
                }

                // shoot delay
                enemy.StartCoroutine(fireCondition(enemyShootDelay));
            }

            // lower ammo
            if (useAmmo) currentAmmo -= useageAmmo;
        }
    #endregion

    /// <summery> co-routines </summery>
    #region co-routines
        /// <summery> by default this is just a wait after firing but this can be modified to be something like standing still <summery>
        public virtual IEnumerator fireCondition(float delay) {
            canShoot = false;
            yield return new WaitForSeconds(delay);
            canShoot = true;
        }
    #endregion

    /// <summery> basic utilities for this and subclasses </summery>
    #region utils
        /// <summery> this is the function i use to get what the player is aiming at </summery>
        /// update this to use a boxcast at some point [o]
        public List<Collider> runHit(Transform character, float offset = 0) {
            Vector3 targetDirection = Vector3.forward + new Vector3(offset, 0, 0);
            RaycastHit[] hits = Physics.RaycastAll(character.position, character.TransformDirection(targetDirection), range);

            if (hits.Length > 0) { 
                List<Collider> cols = new List<Collider>();
                Array.Reverse(hits);

                int count = 0;
                foreach (RaycastHit hit in hits) { 
                    if (hit.collider.gameObject.layer == wallLayerIndex) break;
                    if (count >= pierce && !infinatePierce) break;

                    cols.Add(hit.collider);
                    count++;
                }

                return cols;
            }

            return new List<Collider>();
        }

    #endregion

    /// <summery> basic developer utils </summery>
    #region dev
        /// <summery> reloads the weapon </summery>
        public void reload() {
            currentAmmo = maxAmmo;
        }
    #endregion
}

namespace attack {
    [System.Serializable]
    public class attackRegistration {
        public string name;
        public AT_base attack;
    }

    [System.Serializable]
    public class attackData {
        public int killCount = 0;
        public string name = "";
        public float damageModifier = 1;
        public float lifeStealModifer = 1;

        public attackData() {}
    }
}