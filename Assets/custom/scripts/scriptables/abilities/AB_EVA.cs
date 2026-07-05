using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new ability", menuName = "ability/EVA")]
public class AB_EVA : AB_base {
    [Header("EVA COMP")]
    public GameObject place;

    public override void use(playerController character) {
        if (character.attack.name != "EVA") return;
        if (character.charge < cost) return;
        character.charge -= cost;

        AT_3D.inst.transform.GetComponent<Animator>().Play("ABILITY");
    }

    public static void finishPlacing() {
        Debug.Log("FINISH PLACING :: hi");
        if(!playerController.mainPlayer.ability is AB_EVA) return;

        AB_EVA active = (AB_EVA) playerController.mainPlayer.ability;

        GameObject placedEVA = GameObject.Instantiate(active.place, AT_3D.inst.transform.position + (playerController.mainPlayer.transform.forward * 1f), AT_3D.inst.transform.rotation);

        playerController.mainPlayer.attack.unLoad(playerController.mainPlayer);
        playerController.mainPlayer.attack = null;
        playerController.mainPlayer.ability.end(playerController.mainPlayer);
        playerController.mainPlayer.ability = null;
    }
}