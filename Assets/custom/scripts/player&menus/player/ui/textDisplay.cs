using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class textDisplay : MonoBehaviour {
    public static textDisplay instance;

    [Header("components")]
    public TMP_Text screen;
    public TMP_Text continueMessage;
    public GameObject background;

    [Header("config")]
    [Range(0, 1f)] public float keyDelay = 0.05f;

    [Header("data")]
    public List<sys.Text> textToDisplay = new List<sys.Text>();
    public sys.Text continueText = new sys.Text();

    void Start() {StartCoroutine(type());instance = this;}
    public void addText(List<sys.Text> textToAdd) {foreach (sys.Text item in textToAdd)textToDisplay.Add(item);}

    public IEnumerator type() {
        yield return new WaitUntil(() => GS.live.state.loaded);

        while (true) {
            if (textToDisplay.Count > 0) {
                screen.text = "";
                continueMessage.text = continueText.displayVar(new Dictionary<string, string>());
                background.SetActive(true);

                while (screen.text != textToDisplay[0].localise()) {
                    
                    if (screen.text.Length == textToDisplay[0].localise().Length - 1) screen.text = textToDisplay[0].localise();
                    else screen.text = textToDisplay[0].localise().Substring(0, screen.text.Length + 1);
                    
                    if (eevee.input.Collect("interact", "TD1")) screen.text = textToDisplay[0].localise();

                    yield return new WaitForSecondsRealtime(keyDelay);
                }

                textToDisplay.RemoveAt(0);

                yield return new WaitForSecondsRealtime(0.15f);

                while (screen.text.Length > 0) {
                    screen.text = screen.text.Substring(0, screen.text.Length - 1);
                    if (eevee.input.Collect("interact", "TD1")) screen.text = "";

                    yield return new WaitForSecondsRealtime(keyDelay);
                }
            }
            
            if (textToDisplay.Count == 0) {
                continueMessage.text = "";
                if(background.activeSelf) background.transform.GetComponent<Animator>().Play("close");
                yield return new WaitUntil(() => textToDisplay.Count != 0);
                continueMessage.text = continueText.displayVar(new Dictionary<string, string>());
                background.SetActive(true);
            }
        }
    }
}
