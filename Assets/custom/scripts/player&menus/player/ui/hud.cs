using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class hud : MonoBehaviour {
    [Header("outputs")]
    public TMP_Text speedOutput;
    public TMP_Text weaponOutput;

    // comp
    private Rigidbody rb;
    private playerController player;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        player = transform.GetComponent<playerController>();

        if (speedOutput != null) StartCoroutine(CO_trackSpeed()); // speed
        if (weaponOutput != null) StartCoroutine(CO_trackWeapon()); // current weapon
    }

    /*
        Track speed
    */
    public IEnumerator CO_trackSpeed() {
        while (true) {
            yield return 0;
            speedOutput.text = $"{player.health}/{player.maxHealth} :: {(player.canDash ? "1" : "0")} : {(player.jumpCount)} : {(player.attack.canShoot ? "1" : "0")} :: {Math.Round(rb.linearVelocity.magnitude, 2).ToString()} ku/m-s";
        }
    }

    /*
        Track weapon
    */
    public IEnumerator CO_trackWeapon() {
        while (true) {
            yield return 0;
            weaponOutput.text = $"{player.attack.displayName.localise()} : {player.attack.attackData.name} : {player.attack.attackData.killCount} : charge {player.charge}";
        }
    }

}
