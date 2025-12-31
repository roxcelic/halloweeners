using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/add save")]
public class PM_AddSave : PM_Base {
    public async override void action(pauseMenuController PMC, string input = "") {
        PMC.question(() => {
            save.var.saves.saves.Add(new save.saveData());
            save.data.push();
            
            PMC.reset();
        });
    }
}
