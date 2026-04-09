using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class hiveMind : MonoBehaviour {
    /// <summery> the static instances </summery>
    public static hiveMind instance;
    public static brain target;

    // components
    [Header("comp")]
    public TMP_Text mainText;

    /// <summery> basic mono functions </summery>
    void Start() {
        instance = this;
        mainText = transform.GetComponent<TMP_Text>();
        
        if (target == null) close();
        else open();
    }

    /// <summery> the code to be ran when a new target is set </summery>
    public void open() {
        mainText.text = $"{target.thought}<\n{target.brainT.ToString()}<";
    }

    /// <summery> the code to be ran when the target is empty </summery>
    public void close() {
        mainText.text = "///";
    }

    /// <summery> the code to change the target </summery>
    public static void updateTarget(brain newTarget = null) {
        if (newTarget != null) {
            target = newTarget;
            crosshair.target = target.brainT;
            instance.open();
        } else {
            crosshair.target = brain.brainTypes.empty;
            instance.close();
        }
    }
}