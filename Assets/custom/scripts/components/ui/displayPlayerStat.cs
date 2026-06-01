using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class displayPlayerStat : MonoBehaviour {
    public TMP_Text localText;

    public enum playerStats {
        jumpCount,
        dashCount
    }

    public playerStats selectedStat;

    void Start() {
        if (localText == null) transform.GetComponent<TMP_Text>();
    }

    void Update() {
        if (localText == null) return;

        switch (selectedStat) {
            case playerStats.jumpCount:
                localText.text = $"{playerController.mainPlayer.jumpCount}/{playerController.mainPlayer.maxJumpCount}";
                break;
            case playerStats.dashCount:
                localText.text = $"{(playerController.mainPlayer.canDash ? "1" : "0")}";
                break;
        }
    }
}
