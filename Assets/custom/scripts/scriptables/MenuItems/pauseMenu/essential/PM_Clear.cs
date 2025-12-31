using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/essential/Clear")]
public class PM_Clear : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        PMC.LogDisplay.text = "";
    }
}
