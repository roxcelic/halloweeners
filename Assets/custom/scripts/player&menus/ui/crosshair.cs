using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class crosshair : MonoBehaviour {
    public static brain.brainTypes target = brain.brainTypes.empty;
    public Image display;

    [Header("colors")]
    public Color Interact;
    public Color Enemy;
    public Color Default;

    void Update() {
        display.color = Color.Lerp(display.color, getAppropriateColor(), Time.fixedDeltaTime * 5f);
    }

    private Color getAppropriateColor() {
        Dictionary<brain.brainTypes, Color> colors = new Dictionary<brain.brainTypes, Color>{
            {brain.brainTypes.empty, Default},
            {brain.brainTypes.interactable, Interact},
            {brain.brainTypes.enemy, Enemy}
        };

        return colors[target];
    }
}