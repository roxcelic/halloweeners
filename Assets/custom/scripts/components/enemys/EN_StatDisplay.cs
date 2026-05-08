using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class EN_StatDisplay : MonoBehaviour {
    private TMP_Text text;
    public EN_base target;
    public string output {
        get;
        private set;
    }

    void Start() {
        text = transform.GetComponent<TMP_Text>();
    }

    void Update() {
        if (target == null) return;
    
        output = $"{target.currentHealth}/{target.maxHealth} -- /// ERR: NU";

        if (text != null) text.text = output;
    }
}