using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using ext;

public class attackMenu : MonoBehaviour {
    public List<string> items;
    public int index;
    public int? result;

    public TMP_Text MainDisplay;
    public RectTransform MainDisplayRect;

    [Header("config")]
    public float textHeight = 15f; // this is for the offset when selecting options
    public int ignorance = 5;

    void Update() {
        if (eevee.input.Collect("down", "am")) {index++; if (index > items.Count - 1) index = 0;displayText();}
        if (eevee.input.Collect("up", "am")) {index--; if (index < 0) index = items.Count - 1;displayText();}
    
        if (eevee.input.Collect("interact", "am")) {
            result = index;
        }

        alignTextBox();
    }

    /// <summery> load the values into the menu </summery>
    public void load(List<string> newItems, int newIndex = 0) {
        items = newItems;
        index = newIndex;
        result = null;
        displayText();
    }

    /// <summery> displays the text </summery>
    public string displayText() {
        string result = "";
        
        for (int i = 0; i < items.Count; i++) {
            result += $"{(index == i ? ">" : (i < index ? "|" : ""))} {items[i]} \n";
        }

        MainDisplay.text = result;
        return result;
    }

    /// <summery> a manager </summery>
    public async Task<int> manage(List<string> newItems, int newIndex = 0) {
        load(newItems, newIndex);
        while (result == null) await Task.Delay(50);
        transform.gameObject.SetActive(false);
        return (int)result;
    }

    private void alignTextBox() {MainDisplayRect.localPosition = Vector3.Lerp(MainDisplayRect.localPosition, new Vector3(MainDisplayRect.localPosition.x, Mathf.Clamp(index - ignorance, 0, Mathf.Infinity) * textHeight, MainDisplayRect.localPosition.z), Time.fixedDeltaTime * 5);}
}