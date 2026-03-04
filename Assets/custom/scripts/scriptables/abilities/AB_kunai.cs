using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new ability", menuName = "ability/kunai")]
public class AB_kunai : AB_base {
    /// <summery> set hit </summery>
    public static Transform hit = null;

    [Header("comp")]
    public GameObject kunaiPrefab;
    [Range(0f, 15f)] public float speed = 10f;

    // has the kunai been thrown
    public bool sent = false;
    public bool moving = false;

    private bool breakHook => (
        !eevee.input.Check("Jump") && 
        !eevee.input.Check("Slam") && 
        !eevee.input.Check("Dash")
    );

    #region Main    
        /// <summery> the start function, use to load values etc </summery>
        public override void start(playerController character) {
            sent = false;
            moving = false;
        }

        /// <summery> code ran every frame </summery>
        public override void update(playerController character) {}

        /// <summery> code ran on the end of the scene </summery>
        public override void end(playerController character) {}
        
        /// <summery> the main ability </summery>
        public override void use(playerController character) {
            // cost
            if (character.charge < cost || sent || moving) return;
            character.charge -= cost;

            GameObject kunai = GameObject.Instantiate(kunaiPrefab, new Vector3(), Quaternion.identity);
            kunai.transform.position = character.transform.position;
            
            character.StartCoroutine(thrower(
                () => { sent = true; },
                () => {
                    sent = false;
                    if (kunai != null) kunai.transform.GetComponent<AB_E_kunai>().bullet = false;
                    if (hit != null) character.StartCoroutine(throwPlayer(character, kunai));
                },
                kunai.transform
            ));
        }
    #endregion

    #region functions
    /// <summery> throw the kunai </summery>
    public IEnumerator thrower(System.Action start, System.Action end, Transform target) {
        start();
        yield return new WaitUntil(() => (target == null || hit != null));
        end();
    }

    /// <summery> move the player </summery>
    public IEnumerator throwPlayer(playerController player, GameObject kunai) {
        player.CanMove = false;
        
        // store some stuff
        float ogVel = player.addVel.vel;
        Vector3 ogLVel = player.rb.linearVelocity;

        // reset this shit
        player.addVel.vel = 0;
        player.rb.linearVelocity = new Vector3();

        // Vector3 direction = (hit.position - player.transform.position).normalized;
        Vector3 direction = new Vector3();
        Vector3 hitPoint = hit.position + new Vector3(0, 2, 0);

        while (hit != null && Vector3.Distance(player.transform.position, hitPoint) > 1f && breakHook && canSeeTarget(player.transform, hit.transform)) {
            // player.addVel.AddForce(15f);
            // player.rb.linearVelocity = Vector3.SmoothDamp(player.rb.linearVelocity, player.addVel.getVelocity(player, direction), ref player.Velocity, player.MovementSmoothing);

            // Vector3 direction = (hit.position - player.transform.position).normalized;
            direction = (hitPoint  - player.transform.position).normalized;

            player.transform.position = Vector3.Lerp(player.transform.position, player.transform.position + direction * speed, Time.deltaTime * 5);

            yield return 0;
        }

        // reset this shit
        player.addVel.vel = ogVel;
        player.rb.linearVelocity = ogLVel;

        if (!breakHook) player.addVel.AddForce(15f);
        Destroy(kunai);

        player.CanMove = true;

        hit = null;
    }


    /// <summery> check to see if its in distance </summery>
    public bool canSeeTarget(Transform self, Transform target) {
        return true;
        float maxRange = 150;

        if(Vector3.Distance(self.position, target.position) < maxRange ){
            if(Physics.Raycast(self.position, (target.position - self.position), out RaycastHit hit, maxRange)) {
                if(hit.transform == target) {
                    return true;
                }
            }
        }

        Debug.Log("cant see");

        return false;
    }
    #endregion
}