using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/dev")]
public class PM_Dev : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        // save data
        save.saveData currentSave = save.getData.viewSave();

        // save attack
            currentSave.dev = !currentSave.dev;
            PMC.log($"developer mode has been set to: {currentSave.dev}");

        save.getData.save(currentSave);
    }
}
