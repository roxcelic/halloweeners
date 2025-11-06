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

    void Start() {
        // get textbox
        TMP_Text textBox = transform.GetComponent<TMP_Text>();
        if (textBox == null) return;

        textBox.text = display.displayVar(new Dictionary<string,string>());
    }
}
