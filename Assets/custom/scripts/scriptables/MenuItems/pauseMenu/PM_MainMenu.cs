using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/Main Menu")]
public class PM_MainMenu : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        Time.timeScale = 1f; // reset time
        playerController.mainPlayer.ScreenEffect.Play("fadeOut");

        PMC.StartCoroutine(wait());
    }

    public IEnumerator wait() {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(0);
    }
}
