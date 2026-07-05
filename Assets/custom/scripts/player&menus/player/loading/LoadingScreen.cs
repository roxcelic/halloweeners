using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class LoadingScreen : MonoBehaviour {
    /// <summery> allows the loadingScreen to be accessed easily </summery>
    public static LoadingScreen mainScreen;

    [Header("comp")]
    public TMP_Text text;
    public Animator anim;

    [Header("data")]
    public float completion;
    public string animName;
    public bool active = true;

    [Header("config")]
    [Range(0, 5f)] public float startDelay = 2f;
    public Dictionary<string, bool> loadingItems = new Dictionary<string, bool>();

    [Header("text")]
    public List<sys.Text> startMessage;
    public sys.Text generationMessage = new sys.Text();

    /// <summery> starts the loading </summery>
    void Start() {
        StartCoroutine(load());
        mainScreen = this;
    }
    
    /// <summery> allows you to add / edit a spawn condition  </summery>
    public void spawnCondition(string name, bool condition) {loadingItems[name] = condition;}

    /// <summery> the main loader </summery>
    public IEnumerator load() {
        mainScreen = this;

        yield return new WaitUntil(() => active); // wait for it to become active
        yield return new WaitForSeconds(startDelay); // wait before starting

        // wait for all tasks to be completed
        string firstItemToBeNo = "";
        while (true) {
            firstItemToBeNo = "";
            
            foreach(string item in loadingItems.Keys.ToList()) {
                if (firstItemToBeNo == "" && !loadingItems[item]) firstItemToBeNo = item;
            }

            if (firstItemToBeNo == "") break;
            else text.text = firstItemToBeNo;
            Debug.Log($"loading {firstItemToBeNo}");

            yield return 0;
        }

        // display an appropriate message on the players screen
        while (completion <= 99.99f) {
            if (completion == 0) text.text = generationMessage.localise();
            else text.text = $"{(100 - Math.Round(completion, 2)).ToString()}";
            yield return 0;

        }

        // move to animating the opening sequence
        foreach (char ch in text.text) {
            text.text = text.text.Substring(1, text.text.Length - 1);
            yield return new WaitForSeconds(0.1f);
        }

        // pick a random message to move to
        string chosenMessage = startMessage[UnityEngine.Random.Range(0, startMessage.Count - 1)].localise();

        // display the chosen message
        foreach (char ch in chosenMessage) {
            text.text += ch.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        // wait before playing the animation
        yield return new WaitForSeconds(1f);
        anim.Play(animName);

        // start the waves        
        waveManager playerWave = transform.GetComponent<waveManager>();
        if (playerWave != null) playerWave.Begin();
        
        GS.live.state.loaded = true; // load the game
        playerController.mainPlayer.loaded = true; // load the player
    }
}