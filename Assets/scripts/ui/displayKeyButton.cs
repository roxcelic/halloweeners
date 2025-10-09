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
    void Start() {
        // get textbox
        TMP_Text textBox = transform.GetComponent<TMP_Text>();
        if (textBox == null) return;

        // find text
        string[] words = textBox.text.Split(" ");
        List<string> selectedWords = new List<string>();

        foreach(string word in words) {
            if ((word.Length - 1) > 5 && word.Substring(0, 5) == "!key:") {
                string key = word.Substring(5, word.Length - 5);
                Dictionary<string, eevee.config> FullConfig = eevee.inject.retrieve().FullConfig;
                if (!FullConfig.ContainsKey(key)) selectedWords.Add(word);

                eevee.config selected_input = FullConfig[key];

                switch(eevee.conf.autoDetect()) {
                    case eevee.inputCL.keyboard: 
                        foreach (int keyCode in selected_input.KEYBOARD_code) selectedWords.Add(((KeyCode)keyCode).ToString());

                        break;
                    case eevee.inputCL.controller: 
                        foreach (string buttonCode in selected_input.CONTROLLER_name) selectedWords.Add(buttonCode);

                        break;
                }
            } else {
                selectedWords.Add(word);
            }
        }

        textBox.text = string.Join(" ", selectedWords);
    }
}
