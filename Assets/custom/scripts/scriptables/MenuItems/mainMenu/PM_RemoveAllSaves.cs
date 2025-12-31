using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/reset save")]
public class PM_ResetSave : PM_Base {
    public async override void action(pauseMenuController PMC, string input = "") {
        PMC.question(() => {
            save.var.saves = new save.fullSave();;
            save.data.push();
            
            PMC.reset();
        });
    }
}
