using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

public class editRenderTexture : MonoBehaviour {
    public enum editType {
        width,
        height
    }
    public enum inputType {
        input
    }

    private TMP_InputField LL_input;

    [Header("config")]
    public editType type;
    public inputType input;

    [Header("data")]
    public RenderTexture rt;

    void Start() {
        switch (input) {
            case inputType.input: default: 
                LL_input = transform.GetComponent<TMP_InputField>();
                if (LL_input == null) return;

                switch (type) {
                    case editType.width:
                        LL_input.onValueChanged.AddListener(delegate {rt.width = (Int32)LL_input.getFloatValue();});
                        LL_input.text = rt.width.ToString();

                        break;
                    case editType.height:
                        LL_input.onValueChanged.AddListener(delegate {rt.height = (Int32)LL_input.getFloatValue();});
                        LL_input.text = rt.height.ToString();

                        break;
                }

                break;
        } 
    }
}