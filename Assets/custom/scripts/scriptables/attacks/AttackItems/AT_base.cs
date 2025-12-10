using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/base")]
public class AT_base : ScriptableObject {
        /// <summery> variables </summery>
    #region variables
        [Header("basic values")]
        [Range(0f, 100f)] public float range = 25f;
        [Range(0f, 25f)] public float shootDelay = 1f;
        [Range(0f, 25f)] public float enemyShootDelay = 1f;
        [Range(0f, 25f)] public float nockbackForce = 10f;
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

    // basics
    #region basics
        public virtual void update(playerController character) {}
        public virtual void unLoad(playerController character) {}

        public virtual void load(playerController character, bool reload = true) {
            // animators
            character.AttackDisplay.runtimeAnimatorController = AC;
            character.crosshairDisplay.runtimeAnimatorController = crosshair;

            // data
            canShoot = true;
            if (reload) currentAmmo = maxAmmo;
        }

        public virtual void safeLoad(playerController character) {load(character, false);}
        
        public virtual void enemyLoad(EN_base enemy) {
            // animators
            if (enemyDis) enemy.AttackDisplay.runtimeAnimatorController = enemyDisplay;

            // data
            canShoot = true;
            currentAmmo = maxAmmo;
        }
    #endregion

    // attack
    #region attack

        public virtual void attack(playerController character) {
            if (!canShoot) return; // if the attack cannot be used return
            if (useAmmo && (currentAmmo - useageAmmo) < 0) return; // if the attack uses ammo and the user has no ammo, return

            if (projectile) {
                // effects
                character.ScreenEffect.Play("flash");
                character.AttackDisplay.Play("attack");

                projectileHit(character);
            } else {
                // effects
                character.AttackDisplay.Play("attack");

                // if the attack is run in the animation break here
                if (attackWithAnimation) return;
                hit(character);
            }
        }

        public virtual void extraAttack(playerController character) {
            hit(character);
        }

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
                        player.DealDamage(1, enemy.transform, true, nockbackForce);
                    }   
                }

                // shoot delay
                enemy.StartCoroutine(fireCondition(enemyShootDelay));
            }

            // lower ammo
            if (useAmmo) currentAmmo -= useageAmmo;
        }
    #endregion

    // co-routines
    #region co-routines
        /// <summery> by default this is just a wait after firing but this can be modified to be something like standing still <summery>
        public virtual IEnumerator fireCondition(float delay) {
            canShoot = false;
            yield return new WaitForSeconds(delay);
            canShoot = true;
        }
    #endregion

    // utils
    #region  utils
        public List<Collider> runHit(Transform character, float offset = 0, playerController PC = null) {
            Vector3 targetDirection = Vector3.forward + new Vector3(offset, 0, 0);
            RaycastHit[] hits = PC == null ? Physics.RaycastAll(character.transform.position, character.TransformDirection(targetDirection), range) : Physics.RaycastAll(character.transform.position, PC.camera.TransformDirection(targetDirection), range);

            if (hits.Length > 0) { 
                List<Collider> cols = new List<Collider>();
                Array.Reverse(hits);

                int count = 0;
                foreach (RaycastHit hit in hits) { 
                    cols.Add(hit.collider);
                    count++;

                    if (hit.collider.gameObject.layer == wallLayerIndex) break;
                    if (count >= pierce && !infinatePierce) break;
                }

                return cols;
            }

            return new List<Collider>();
        }

        public Vector3 getAimedLocation(Transform character, playerController PC = null) {
            Vector3 targetDirection = Vector3.forward;
            RaycastHit hit;
            Ray r;

            if (PC == null) {
                r = new Ray(character.transform.position, character.TransformDirection(targetDirection));
                if (Physics.Raycast(character.transform.position, character.TransformDirection(targetDirection), out hit, range, LayerMask.GetMask("Ground"))) {
                    return hit.point;
                }
            } else {
                r = new Ray(character.transform.position, PC.camera.TransformDirection(targetDirection));
                if (Physics.Raycast(character.transform.position, PC.camera.TransformDirection(targetDirection), out hit, range, LayerMask.GetMask("Ground"))) {
                    return hit.point;
                }
            }

            return r.GetPoint(range);
        }

        public void hit(playerController character) {
            // attack itself
            List<Collider> hits = runHit(character.transform, 0, character);
        
            // do something with the attack
            foreach (Collider hit in hits) {
                EN_base enemey = null;

                character.AS.clip = SF_fire;
                character.AS.Play();

                if ((enemey = hit.transform.GetComponent<EN_base>()) != null) {
                    // sound
                    if (enemey.DealDamage((int)(damage * attackData.damageModifier), character.transform, true, nockbackForce)) {
                        attackData.killCount++;
                        character.heal((int)(1 * attackData.lifeStealModifer));
                    }

                } else if (hit.transform.gameObject.tag == AT_fortniteBuild.fortniteTag || hit.transform.gameObject.tag == AT_minecraftCreativeMode.minecraftTag) {
                    Destroy(hit.transform.gameObject);
                }
            }

            // shoot delay
            character.StartCoroutine(fireCondition(shootDelay));

            // lower ammo
            if (useAmmo) currentAmmo -= useageAmmo;
            character.hud.displayText($"{currentAmmo}/{maxAmmo}", Color.red);
        }

        public void projectileHit(playerController character) {
            GameObject tmpObj = Instantiate(projectilePrefab, character.transform.position + (character.transform.forward * 2), Quaternion.identity);
            tmpObj.transform.GetComponent<Rigidbody>().AddForce((character.transform.forward * projectileForce) + new Vector3(0, 20, 0));
            tmpObj.transform.GetComponent<damageOnHit>().attributeKill = character;
        
            character.StartCoroutine(fireCondition(shootDelay));
            if (useAmmo) currentAmmo -= useageAmmo;
            character.hud.displayText($"{currentAmmo}/{maxAmmo}", Color.red);
        }

        /// <summery> a util function to open a menu and return a selected inedx </summery>
        public async Task<int> openMenu(List<string> newItems, int newIndex = 0) {
            GS.live.state.menu(true);
            attackMenu AM = Instantiate(Resources.Load<GameObject>("weapons/general/attackMenu")).transform.GetComponent<attackMenu>();
            AM.transform.gameObject.SetActive(true);

            int result = await AM.manage(newItems, newIndex);

            GS.live.state.menu(false);
            return result;
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