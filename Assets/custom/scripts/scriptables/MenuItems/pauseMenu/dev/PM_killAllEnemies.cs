using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using player.utils;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/kill all enemies")]
public class PM_killAllEnemies : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Enemy")) {
            PMC.log($"killing: {item}", sys.programNames.system.localise(), "blue");
            Destroy(item);
        }
    }
}