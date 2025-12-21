using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

/*
    !key:name
*/
public class displayKeyButton : MonoBehaviour {
    public sys.Text display = new sys.Text();
    [Range(0, 5f)] public float delay = 1.5f;
    private float inteval = 0.5f;
    private TMP_Text textBox;

    void Start() {
        // get textbox
        textBox = transform.GetComponent<TMP_Text>();
        if (textBox == null) return;

        textBox.text = display.displayVar(new Dictionary<string,string>());
    }
}
