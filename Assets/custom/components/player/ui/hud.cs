using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class hud : MonoBehaviour {
    [Header("outputs")]
    public TMP_Text speedOutput;
    public TMP_Text weaponOutput;
    public TMP_Text dashOutput;
    public TMP_Text killCountOutput;

    // comp
    private Rigidbody rb;
    private playerController player;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        player = transform.GetComponent<playerController>();

        // begin tracking
        if (speedOutput != null) StartCoroutine(CO_trackSpeed()); // speed
        if (weaponOutput != null) StartCoroutine(CO_trackWeapon()); // current weapon
        if (dashOutput != null) StartCoroutine(CO_trackDash()); // dash
        if (killCountOutput != null) StartCoroutine(CO_killCount()); // kill count
    }

    /*
        Track speed
    */
    public IEnumerator CO_trackSpeed() {
        while (true) {
            yield return 0;
            speedOutput.text = $"{Math.Round(rb.linearVelocity.magnitude, 2).ToString()} km/s";
        }
    }

    /*
        Track weapon
    */
    public IEnumerator CO_trackWeapon() {
        while (true) {
            yield return 0;
            weaponOutput.text = $"{player.attack.displayName.localise()}";
        }
    }

    /*
        Track dash
    */
    public IEnumerator CO_trackDash() {
        while (true) {
            yield return 0;
            dashOutput.text = $"{(player.canDash ? "1" : "0")} :: {(player.jumpCount)} :: {(player.attack.canShoot ? "1" : "0")}";
        }
    }

    /*
        Track kill count
    */
    public IEnumerator CO_killCount() {
        while (true) {
            yield return 0;
            killCountOutput.text = $"{player.attack.attackData.name} : {player.attack.attackData.killCount} : {player.attack.liveKills}";
        }
    }


    
}
