using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class textDisplay : MonoBehaviour {
    public static textDisplay instance;

    [Header("components")]
    public TMP_Text screen;
    public TMP_Text screenDisplay;

    public TMP_Text continueMessage;
    public GameObject background;

    [Header("config")]
    [Range(0, 1f)] public float keyDelay = 0.05f;
    public bool waitUntilPlayerMovement = true;

    [Header("data")]
    public List<sys.Text> textToDisplay = new List<sys.Text>();
    public sys.Text continueText = new sys.Text();

    void Start() {StartCoroutine(type());instance = this;}
    public void addText(List<sys.Text> textToAdd) {foreach (sys.Text item in textToAdd)textToDisplay.Add(item);}

    public IEnumerator type() {
        yield return new WaitUntil(() => GS.live.state.loaded);
        if (waitUntilPlayerMovement) yield return new WaitUntil(() => GS.live.state.moved);

        while (true) {
            if (textToDisplay.Count > 0) {
                screen.text = "";
                continueMessage.text = continueText.displayVar(new Dictionary<string, string>());
                if(background != null) background.SetActive(true);

                string displayText = "";
                while (displayText != $"{textToDisplay[0].localise()}") {
                    
                    if (displayText.Length == textToDisplay[0].localise().Length - 1) displayText = textToDisplay[0].localise();
                    else displayText = textToDisplay[0].localise().Substring(0, displayText.Length + 1);
                    
                    if (eevee.input.Collect("interact", "TD1")) displayText = textToDisplay[0].localise();

                    screen.text = $"<i>[</i>{displayText}<i>]</i>";
                    screenDisplay.text = screen.text;

                    yield return new WaitForSecondsRealtime(keyDelay);
                }

                textToDisplay.RemoveAt(0);

                float passedTime = 0;
                while (passedTime < 4 && !eevee.input.Collect("interact", "TD1")) {
                    passedTime += Time.deltaTime;
                    yield return 0;
                }

                while (displayText.Length > 0) {
                    displayText = displayText.Substring(0, displayText.Length - 1);
                    if (eevee.input.Collect("interact", "TD1")) displayText = "";

                    screen.text = $"<i>[</i>{displayText}<i>]</i>";
                    screenDisplay.text = screen.text;

                    yield return new WaitForSecondsRealtime(keyDelay);
                }

                screen.text = "";
                screenDisplay.text = screen.text;
            }
            
            if (textToDisplay.Count == 0) {
                continueMessage.text = "";
                if(background != null &&background.activeSelf) background.transform.GetComponent<Animator>().Play("close");
                yield return new WaitUntil(() => textToDisplay.Count != 0);
                continueMessage.text = continueText.displayVar(new Dictionary<string, string>());
                if(background != null) background.SetActive(true);
            }
        }
    }
}
