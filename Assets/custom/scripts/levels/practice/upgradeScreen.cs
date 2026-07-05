using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class upgradeScreen : MonoBehaviour {
    public static upgradeScreen instance;

    [Header("comp")]
    public CanvasGroup CG;
    public List<Button> abilitySlots;

    // scoreboard
    public GameObject board;
    public typeOutText SB_type;
    public float points = 0;

    void Start() {
        instance = this; // setup the instance variable
    }

    /// <summery> finds the upgrades possible </summery>
    public void findPossibleUpgrades() {StartCoroutine(loadMenu(new List<stats.config.upgradeTypes>(playerController.mainPlayer.attack.stat.upgradeAbilities)));}

    /// <summery> the main animation </summery>
    public IEnumerator loadMenu(List<stats.config.upgradeTypes> acceptedTypes) {
        GS.live.state.menued = true;
        Cursor.lockState =  CursorLockMode.None;
        Cursor.visible = true;
        playerController.mainPlayer.CanMove = false;

        // fade in the screen
        CG.blocksRaycasts = true;
        CG.interactable = true;
        while(CG.alpha <= 0.99f) {
            CG.alpha = Mathf.Lerp(CG.alpha, 1, Time.fixedDeltaTime * 5f);
            yield return 0;
        }
        CG.alpha = 1f;

        // calc points
        points = (Int32)(PlayerPrefs.GetInt("difficulty", 0) + playerController.mainPlayer.transform.GetComponent<waveManager>().baseWaves / 10);

        // display the options
        System.Random rnd = new System.Random();
        foreach(Button button in abilitySlots) {
            if(acceptedTypes.Count == 0) Destroy(button.gameObject);
            else {
                stats.config.upgradeTypes type = acceptedTypes[rnd.Next(0, acceptedTypes.Count - 1)];
                acceptedTypes.Remove(type);

                button.transform.gameObject.SetActive(true);    
                button.transform.GetComponent<updgradeButton>().upgrade = type;
                button.transform.GetComponent<updgradeButton>().screen = this;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = type.ToString();

                board.SetActive(true);
                SB_type.type(generateFinishMessage());
            }
        }
    }

    /// <summery> just get the score n stuff </summery>
    public string generateFinishMessage() {
        return $@"
map: {PlayerPrefs.GetString("levelType", "???")}
difficulty: {PlayerPrefs.GetInt("difficulty", 0)}
wavesPassed: {PlayerPrefs.GetInt("difficulty", 0) + playerController.mainPlayer.transform.GetComponent<waveManager>().baseWaves}
points: {points}
finishedUsingWeapon: {playerController.mainPlayer.attack.name} // {playerController.mainPlayer.attack.attackData.name}
        ";
    }

    /// <sumery> allows the user to submit an upgrade </summery>
    public void Submit(stats.config.upgradeTypes type) {
        if (points <= 0) {
            SB_type.type($"no points left");
            
            Time.timeScale = 1f; // reset time
            transform.GetComponent<Animator>().Play("fadeOut");

            StartCoroutine(wait());
            
            return;
        }
        SB_type.type($"upgrading {type.ToString()} on {playerController.mainPlayer.attack.name} by {1} points \n points left: {points - 1}");
        
        playerController.mainPlayer.attack.attackData.statData.damageBonus += 1f;
        playerController.mainPlayer.quicksave();
        
        points -= 1f;
    }

    public IEnumerator wait() {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(0);
    }
}