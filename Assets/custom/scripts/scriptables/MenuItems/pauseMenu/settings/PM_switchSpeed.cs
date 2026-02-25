using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/switch speed")]
public class PM_switchSpeed : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
         saveData currentSave = getData.viewSave();

        switch(currentSave.speedAnimation) {
            case 1:
                currentSave.speedAnimation = 2;

                break;
            case 2: default:
                currentSave.speedAnimation = 1;

                break;
        }

        getData.save(currentSave);
        MS_matchSpeed.instance.UpdateSpeed();
    }
}
