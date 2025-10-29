using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/Main Menu")]
public class PM_MainMenu : PM_Base {
    public override void action(pauseMenuController PMC) {
        Time.timeScale = 1f; // reset time
        SceneManager.LoadScene(0);
    }
}
