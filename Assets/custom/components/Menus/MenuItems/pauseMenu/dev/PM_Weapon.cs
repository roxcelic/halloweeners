using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/weapon")]
public class PM_weapon : PM_Base {
    public AT_base attack;

    public async override void action(pauseMenuController PMC, string input = "") {
        playerController.mainPlayer.switchAttack(attack);
        PMC.log($"weapon switched to {attack.displayName.localise()}");
    }
}