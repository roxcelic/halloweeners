using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class loopText : MonoBehaviour {
    public string text = "test";
    public int textAmount = 500;

    TMP_Text mainTextComp;
    RectTransform RT;

    void Start() {
        mainTextComp = transform.GetComponent<TMP_Text>(); // get the text comp
        RT = transform.GetComponent<RectTransform>();

        changeText(text);

        StartCoroutine(scroll());
    }

    public void changeText(string input) {
        text = input;

        mainTextComp.text = $"\t{string.Concat(System.Linq.Enumerable.Repeat(text, textAmount))}\t";
    }

    public IEnumerator scroll() {
        float Height = mainTextComp.preferredHeight;
        Vector3 startPosition = RT.position;

        float scrollPosition = 0;

        while (true) {
            RT.position = new Vector3(startPosition.x, -scrollPosition % (mainTextComp.preferredHeight / 2), startPosition.z);
            scrollPosition += 5 * 20 * Time.deltaTime;

            yield return 0;
        }
    }
}