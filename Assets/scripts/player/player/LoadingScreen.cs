using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class LoadingScreen : MonoBehaviour {
    public float completion;
    public TMP_Text text;
    public Animator anim;
    public string animName;
    public bool active = true;
    public List<string> startMessage;
    private playerController PC;
    
    void Start() {
        PC = transform.GetComponent<playerController>();
    }

    void Update() {
        if (!active) return;

        if (completion == 0) text.text = "generating map.";
        else text.text = $"{(100 - Math.Round(completion, 2)).ToString()}";

        if (completion >= 95.5f) {
            StartCoroutine(go());

            active = false;
        }
    }

    public IEnumerator go() {
        
        foreach (char ch in text.text) {
            text.text = text.text.Substring(1, text.text.Length - 1);
            yield return new WaitForSeconds(0.1f);
        }

        string chosenMessage = startMessage[UnityEngine.Random.Range(0, startMessage.Count - 1)];

        foreach (char ch in chosenMessage) {
            text.text += ch.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        anim.Play(animName);
        transform.GetComponent<waveManager>().Begin();
        PC.loaded = true;
    }
}
