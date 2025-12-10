using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class mainMenuController : MonoBehaviour {
    
    [Header("components")]
    public TMP_Text mainText;
    public GameObject popUp;

    [Header("items")]
    public List<MM_base> currentItems;
    public List<List<MM_base>> previousItems = new List<List<MM_base>>();

    [Header("config")]
    public char selectedItem;

    [Header("data")]
    public int selected;
    public bool canMove = true;
    private Coroutine animating;
    public sys.Text headingMessage = new sys.Text();

    void Start() {
        animating = StartCoroutine(textAnim(loadText(selected = 0)));
        canMove = true;
    }

    void Update() {
        if (!canMove) return;

        // up
        if (eevee.input.Collect("up")) {
            if (selected <= 0) selected = currentItems.Count - 1;
            else selected--;

            reload();
        }

        // down
        if (eevee.input.Collect("down")) {
            if (selected >= currentItems.Count - 1) selected = 0;
            else selected++;

            reload();
        }

        // interact
        if (eevee.input.Collect("interact")) {
            currentItems[selected].action(this);

            reload();
        }

        // back
        if (eevee.input.Collect("back")) loadPrevMenu();
    }

    /*
        load next menu
    */
    public void loadNextMenu(List<MM_base> menuItems) {
        if (menuItems.Count == 0) return;

        previousItems.Add(currentItems);
        currentItems = menuItems;
        selected = 0;

        reload();
    }

    public void loadPrevMenu() {
        currentItems = previousItems[previousItems.Count - 1];
        selected = 0;
        reload();
    }

    /*
        Load text
    */ 
    public string loadText(int selected) {
        string finalText = headingMessage.displayVar(new Dictionary<string, string>());

        for (int i = 0; i <= currentItems.Count - 1; i++) {
            if (i == selected) finalText += $"{selectedItem.ToString()} {currentItems[i].display.localise()} --- {i}\n";
            else finalText += $"{currentItems[i].display.localise()} --- {i}\n";
        }

        return finalText;
    }

    /*
        reload menu
    */
    public void reload() {
        if (animating != null) StopCoroutine(animating);
        mainText.text = loadText(selected);
        animating = StartCoroutine(textAnim(loadText(selected)));
    }

    /*
        Text anim
    */
    public IEnumerator textAnim(string text, float delayPerKey = 0.01f) {
        mainText.text = "";

        while (mainText.text != text) {
            mainText.text += text[mainText.text.Length];
            yield return new WaitForSeconds(delayPerKey);
        }
    }
}
