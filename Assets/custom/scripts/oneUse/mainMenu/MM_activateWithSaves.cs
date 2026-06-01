using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/activate with save")]
public class MM_activateWithSaves : PM_Base {
    public override bool active() {
        fullSave currentSave = data.getSaves();
        bool foundSave = false;

        foreach (saveData save in currentSave.saves) foundSave = foundSave || save.name != "";
        
        return foundSave;
    }
}