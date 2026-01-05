using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/delete save")]
public class PM_DeleteSave : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        PMC.question(() => {
            save.var.saves.saves.RemoveAt(PlayerPrefs.GetInt("saveSlot", 0));
            save.data.push();

            PMC.reset();
        });
    }
}
