using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

namespace player {
    
}

namespace movement {
    [System.Serializable]
    public class additionalVelocity {
        public float vel = 0;
        public float maxVel = 40f;
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