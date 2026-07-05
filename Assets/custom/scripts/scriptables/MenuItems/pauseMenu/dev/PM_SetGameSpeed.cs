using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/set game speed")]
public class PM_SetGameSpeed : PM_Base {
    public async override void action(pauseMenuController PMC, string input = "") {
        GS.live.state.gameSpeed = float.Parse(await PMC.getText(GS.live.state.gameSpeed.ToString()));
    }
}