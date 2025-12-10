using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

public class NEN_healPet : NEN_base {
    [Header("pet stuff")]
    public Vector3 offset;
    
    // main rubish
    public override void begin() {StartCoroutine(movement());}

    public override IEnumerator movement() {
        while (true) {
            if (playerController.mainPlayer != null) transform.position = offset + playerController.mainPlayer.transform.position;

            yield return 0;
        }
    }
}