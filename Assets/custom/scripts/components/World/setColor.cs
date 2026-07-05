using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

namespace colorManager {
    public static class data{
        public static bool useChosenColor = true;
        public static Color worldColor;

        public static Color targetColor => useChosenColor ? save.utils.getColor() : worldColor;

        public static void forceColor(Color forcedColor) {
            useChosenColor = false;
            worldColor = forcedColor;
        }
        public static void freeColor() {
            useChosenColor = true;
        }
    }
}

public class setColor : MonoBehaviour {
    public Material M_worldMat;
    public typeOfData affect;
    public bool active = true;
    public bool allowCutsomColor = false;
    public bool invertColor = false;

    public enum typeOfData {
        mat,
        image,
        text,
        baseMat,
        spriteRenderer,
        light
    }

    // comp
    private Image image;
    private SpriteRenderer Sr;
    private TMP_Text text;
    private Light light;

    void Start(){
        image = transform.GetComponent<Image>();
        text = transform.GetComponent<TMP_Text>();
        Sr = transform.GetComponent<SpriteRenderer>();
        light = transform.GetComponent<Light>();
    }

    void Update(){
        allowCutsomColor = !colorManager.data.useChosenColor;

        if (!active) return;

        Color selectedColor = colorManager.data.targetColor;
        if (invertColor) selectedColor = invert(selectedColor);

        switch (affect) {
            case typeOfData.mat:
                M_worldMat.SetColor("_outlineColor", Color.Lerp(M_worldMat.GetColor("_outlineColor"), selectedColor, Time.fixedDeltaTime * 5f));
                break;
            case typeOfData.baseMat:
                M_worldMat.SetColor("_color", Color.Lerp(M_worldMat.GetColor("_color"), selectedColor, Time.fixedDeltaTime * 5f));
                break;
            case typeOfData.image:
                image.color = Color.Lerp(image.color, selectedColor, Time.fixedDeltaTime * 5f);
                break;
            case typeOfData.text:
                text.color = Color.Lerp(text.color , selectedColor, Time.fixedDeltaTime * 5f);
                break;
            case typeOfData.spriteRenderer:
                Sr.color =  Color.Lerp(Sr.color , selectedColor, Time.fixedDeltaTime * 5f);
                break;
            case typeOfData.light:
                light.color = Color.Lerp(light.color , selectedColor, Time.fixedDeltaTime * 5f);
                break;
        }
    }

    void OnEnable() {
        ChangeColor(colorManager.data.targetColor);
    }

    public void ChangeColor(Color newColor) {
        if (invertColor) newColor = invert(newColor);

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
            case typeOfData.light:
                transform.GetComponent<Light>().color = newColor;
                break;
        }
    }

    public Color invert(Color color) {
        Color.RGBToHSV(color, out float H, out float S, out float V);
        float negativeH = (H + 0.5f) % 1f;
        
        return Color.HSVToRGB(negativeH, S, V);
    }

    //Color.Lerp
}