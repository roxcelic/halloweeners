using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/essential/back")]
public class PM_Back : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        PMC.loadPrevMenu();
    }
}
