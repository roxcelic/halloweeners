using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/copyDir")]
public class PM_copyDir : PM_Base {

    public override void action(pauseMenuController PMC, string input = "") {
        GUIUtility.systemCopyBuffer = Application.persistentDataPath;
    }
}
