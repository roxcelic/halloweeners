using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

public class NEN_willaim : NEN_base {
    [Header("dev settings")]
    public bool DEV_Freeze = false;
    public string DEV_LockAnim = "shoot";

    [Header("willaim settings")]
    [Range(0, 10f)] public float positionStateDelay = 3.5f;
    [Range(0, 25f)] public float spawnRadius = 10f;
    [Range(0, 5f)] public float groundCheckDistance = 2f;


    private int positionState = 10;
    public bool seenPlayer = false;
    public bool able = true;

    [Header("bullet settings")]
    public GameObject bullet;
    public Transform spawnLoc;

    // a whole nothing burger
    protected override void Start() {
        self = transform.GetComponent<EN_base>();
    }

    void OnEnable() {
        if (DEV_Freeze) {
            sys.utils.waiting.waitUntil(() => {
                self.anim.Play(DEV_LockAnim);
            }, () => self != null);
        }        
    }

    public override void begin() {
        placeOnGround();

        if (DEV_Freeze) {
            sys.utils.waiting.waitUntil(() => {
                self.anim.Play(DEV_LockAnim);
            }, () => self != null);
        } else {
            StartCoroutine(changeState());
        }
    }

    // the movement yay
    public IEnumerator changeState() {        
        yield return new WaitUntil(() => self != null);
        stateMan();
        while (!self.dead) {
            yield return new WaitForSeconds(positionStateDelay);
            yield return new WaitUntil(() => canSeePlayer() || positionState == 2);

            stateMan();
        }
    }

    private void stateMan() {
        if (able && !self.dead){
            switch (positionState) {
                case 0: default:
                    positionState = 1; 
                    self.canBeDamaged = true;
                    self.anim.Play("spawn");
                    break;
                case 1: 
                    positionState = 2; 
                    self.anim.Play("shoot");
                    break;
                case 2:
                    positionState = 0; 
                    self.canBeDamaged = false;
                    self.anim.Play("unSpawn");
                    break;
            }     
        }
    }

    public IEnumerator move() {
        Vector3 chosenLocation = new Vector3();

        able = false;
        while (!checkPosition(chosenLocation = playerController.mainPlayer.transform.localPosition + new Vector3(UnityEngine.Random.Range(-spawnRadius, spawnRadius), playerController.mainPlayer.transform.position.y, UnityEngine.Random.Range(-spawnRadius, spawnRadius)))) {
            yield return new WaitForSeconds(1f);
            Debug.Log($"failed to find position: {chosenLocation}");
        }
        Debug.Log("found position");
        able = true;

        transform.position = chosenLocation;
        placeOnGround();
    }

    public bool canSeePlayer() {
        return Vector3.Distance(playerController.mainPlayer.transform.position, transform.position) <= self.sight;
    }

    public void setPosition() {StartCoroutine(move());}

    public bool checkPosition(Vector3 position) {

        if (Physics.Raycast(position, transform.TransformDirection(Vector3.down), out RaycastHit hit, groundCheckDistance)) {
            if (hit.collider.gameObject.layer != sys.var.layers.ground && hit.collider.gameObject.layer != sys.var.layers.ingoreRPGround) {
                Debug.Log($"layer found was {hit.collider.gameObject.layer} not {sys.var.layers.ground}");
                return false; 
            }

            return true;
        }

        return false;
    }

    public void placeOnGround() {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, groundCheckDistance)) {
            transform.position = new Vector3(transform.position.x, hit.point.y + 1.5f, transform.position.z);
        }
    }

    public void spawnBullet() {
        GameObject shot = Instantiate(bullet, spawnLoc.position, Quaternion.identity);
        shot.transform.GetComponent<damageOnHit>().direction = transform.forward;
    }
}
