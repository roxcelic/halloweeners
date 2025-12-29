using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/open scene")]
public class PM_OpenScene : PM_Base {
    [Header("open scene")]
    public string sceneName = "";
    public string id = "";
    public bool unlockedByDefault = true;

    public override void onLoad(pauseMenuController PMC) {
        name.overrideName = "";
        name.overrideName = $"<color=#{(!save.getData.viewSave().unlockedLevels.Contains(id) && !unlockedByDefault ? "828282" : "fff")}>{name.localise()}</color>";
    }

    public override void action(pauseMenuController PMC, string input = "") {
        if (!save.getData.viewSave().unlockedLevels.Contains(id) && !unlockedByDefault) return;

        Time.timeScale = 1f; // reset time
        playerController.mainPlayer.ScreenEffect.Play("fadeOut");
        PMC.StartCoroutine(wait());
    }

    public IEnumerator wait() {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(sceneName);
    }
}
