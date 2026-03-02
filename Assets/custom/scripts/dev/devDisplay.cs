using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using save;

public class devDisplay : MonoBehaviour {
    private TMP_Text display;
    
    void Start() {display = transform.GetComponent<TMP_Text>();}
    
    void Update() {
        if (display == null || !getData.isDev()) {
            if(display != null) display.text = "";
            return;
        }

        display.text = $"{Mathf.Round(1.0f / Time.deltaTime)} :: {getData.viewSave().name} :: {Application.persistentDataPath}";
    }}
