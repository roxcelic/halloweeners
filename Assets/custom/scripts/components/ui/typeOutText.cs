using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class typeOutText : MonoBehaviour {
    private Coroutine typer;
    public TMP_Text mainText;
    public sys.Text startMessage = new sys.Text();

    [Header("config")]
    [Range(0.05f, 5f)] public float textDelay = 0.05f;

    void Start() {if (startMessage != new sys.Text())type(startMessage.localise());}
    void OnEnable(){if (startMessage != new sys.Text())type(startMessage.localise());}

    /// <summery> animate the text typing </summery>
    public void type(string input) {
        if (typer != null) StopCoroutine(typer);
        typer = StartCoroutine(typeAnim(input));
    }

    /// <summery> the anim itself </summery>
    public IEnumerator typeAnim(string input) {
        mainText.text = "";
        while(mainText.text.Length < input.Length) {
            mainText.text = $"{input.Substring(0, mainText.text.Length)}{genString(1)}";
            yield return new WaitForSecondsRealtime(textDelay);
        }
        mainText.text = input;
    }

    /// <summery> a util to make a random string </summery>
    private string genString(int size) {
        System.Random random = new System.Random();

        return new string(Enumerable.Repeat(sys.var.keywords.characters, size)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}