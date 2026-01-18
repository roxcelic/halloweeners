using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class newHud : MonoBehaviour {
    [Header("health")]
    public TMP_Text health;
    public TMP_Text maxHealth;

    [Header("charge")]
    public TMP_Text charge;

    [Header("speed")]
    public TMP_Text speed;

    // comp
    private Rigidbody rb;
    private playerController player;

    void Start() {
        rb = transform.GetComponent<Rigidbody>();
        player = transform.GetComponent<playerController>();

        StartCoroutine(CO_trackDisplays());
    }

    public IEnumerator CO_trackDisplays() {
        while (true) {
            health.text = $"hp:{player.health}/{player.maxHealth}";
            charge.text = $"c:{player.charge}";

            speed.text = $"{Math.Round(rb.linearVelocity.magnitude, 2).ToString()}u/m-s";

            yield return 0;
        }
    }
}