using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/essential/list")]
public class PM_List : PM_Base {
    [Header("list data")]
    public sys.Text currentOptions;

    public override void action(pauseMenuController PMC, string input = "") {
        PMC.log("-----", "", "white");
        PMC.log(currentOptions.localise(), "system", "blue");
        foreach (PM_Base Mitem in PMC.orderCommands()) {
            if (!Mitem.dev || save.getData.isDev()) PMC.log($"\t{Mitem.name.localise()}", sys.programNames.system.localise(), "blue");
        }
        PMC.log("-----", "", "white");
    }
}
