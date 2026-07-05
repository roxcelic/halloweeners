using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class displayWhilePressed : MonoBehaviour {
    public string key = "";
    private CanvasGroup CG;
    private bool locked = false;

    void Start() {
        CG = transform.GetComponent<CanvasGroup>();
    }

    void Update() {
        if (locked) {
            CG.alpha = Mathf.Lerp(CG.alpha, 1, Time.deltaTime * 5f);
        } else {
            CG.alpha = Mathf.Lerp(CG.alpha, eevee.input.Check(key) ? 1 : 0, Time.deltaTime * 5f);
        }
    }

}
