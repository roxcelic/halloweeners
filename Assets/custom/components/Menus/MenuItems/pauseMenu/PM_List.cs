using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/essential/list")]
public class PM_List : PM_Base {
    public override void action(pauseMenuController PMC) {
        string log = "";

        foreach (PM_Base Mitem in PMC.orderCommands()) {
            log += $"{Mitem.name} ,";
        }

        PMC.log(log, "list");
    }
}
