using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/practice/load")]
public class PM_practice : PM_Base {
    public string sceneName = "";

    public override void action(pauseMenuController PMC, string input = "") {
        // select the difficulty
        PM_practiceOptions difficultyHolder = findFirstDifficultyOption(PMC);
        if (difficultyHolder == null) {
            PMC.log("/////////////////// PM_practiceOptions refrence exception", sys.programNames.system.localise(), "red");
            return;
        }
        PlayerPrefs.SetInt("difficulty", difficultyHolder.selected.refrence);

        // select the map
        PM_practiceOptions mapHolder = findFirstMapOption(PMC);
        if (mapHolder == null) {
            PMC.log("/////////////////// PM_practiceOptions refrence exception", sys.programNames.system.localise(), "red");
            return;
        }
        PlayerPrefs.SetString("levelType", difficultyHolder.selected.refrenceName);

        // load the first level
        Time.timeScale = 1f; // reset time
        PMC.interactable = false;
        playerController.mainPlayer.ScreenEffect.Play("fadeOut");
        PMC.StartCoroutine(wait());
        PMC.StartCoroutine(yap(1.1f, PMC));
    }

    // get the first difficulty option loaded in the pause menu
    private PM_practiceOptions findFirstDifficultyOption(pauseMenuController PMC) {
        foreach(PM_Base item in PMC.currentItems) if (item.GetType() == typeof(PM_practiceOptions) && ((PM_practiceOptions)item).options == PM_practiceOptions.optionsType.difficulty) return (PM_practiceOptions)item;
        return null;
    }

    // get the first difficulty option loaded in the pause menu
    private PM_practiceOptions findFirstMapOption(pauseMenuController PMC) {
        foreach(PM_Base item in PMC.currentItems) if (item.GetType() == typeof(PM_practiceOptions) && ((PM_practiceOptions)item).options == PM_practiceOptions.optionsType.map) return (PM_practiceOptions)item;
        return null;
    }

    ///
    /// COMPLETLY STOLEN FROM ANOTHER SCRIPT OF MINE :) 
    ///     PM_OpenScene
    ///
    public IEnumerator wait() {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator yap(float duration, pauseMenuController PMC) {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            PMC.log(genString(67), "", "red", false);

            elapsedTime += Time.fixedDeltaTime;
            yield return 0;
        }
    }

    /// <summery> a util to make a random string </summery>
    private string genString(int len) {
        System.Random random = new System.Random();

        return new string(Enumerable.Repeat(sys.var.keywords.characters, len)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}