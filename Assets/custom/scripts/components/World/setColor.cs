using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

namespace colorManager {
    public static class data{
        public static Color targetColor;

        static data() {targetColor = save.utils.getColor();}

        public static void forceColor(Color forcedColor) {targetColor = forcedColor;}
        public static void freeColor() {targetColor = save.utils.getColor();}
    }
}

public class setColor : MonoBehaviour {
    public Material M_worldMat;
    public typeOfData affect;

    public enum typeOfData {
        mat,
        image,
        text,
        baseMat
    }

    // comp
    private Image image;
    private TMP_Text text;

    void Start(){
        image = transform.GetComponent<Image>();
        text = transform.GetComponent<TMP_Text>();
    }

    void Update(){
        switch (affect) {
            case typeOfData.mat:
                M_worldMat.SetColor("_outlineColor", Color.Lerp(M_worldMat.GetColor("_outlineColor"), colorManager.data.targetColor, Time.fixedDeltaTime * 5f));
                break;
            case typeOfData.baseMat:
                M_worldMat.SetColor("_color", Color.Lerp(M_worldMat.GetColor("_color"), colorManager.data.targetColor, Time.fixedDeltaTime * 5f));
                break;
            case typeOfData.image:
                image.color = Color.Lerp(image.color, colorManager.data.targetColor, Time.fixedDeltaTime * 5f);
                break;
            case typeOfData.text:
                text.color = Color.Lerp(text.color , colorManager.data.targetColor, Time.fixedDeltaTime * 5f);
                break;
        }
    }

    void OnEnable() {
        ChangeColor(colorManager.data.targetColor);
    }

    public void ChangeColor(Color newColor) {
        switch (affect) {
            case typeOfData.mat:
                M_worldMat.SetColor("_outlineColor", newColor);
                break;
            case typeOfData.baseMat:
                M_worldMat.SetColor("_color", Color.Lerp(M_worldMat.GetColor("_color"), colorManager.data.targetColor, Time.fixedDeltaTime * 5f));
                break;
            case typeOfData.image:
                transform.GetComponent<Image>().color = newColor;
                break;
            case typeOfData.text:
                transform.GetComponent<TMP_Text>().color = newColor;
                break;
        }
    }

    //Color.Lerp
}