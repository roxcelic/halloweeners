using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class PM_ViewStats : PM_Base {
    public AVdata.savedAttack loadedAttack;

    public override void action(pauseMenuController PMC, string input = "") {
        string statsText = $@"
Weapon Type: {loadedAttack.attackName}
";
        PMC.log(statsText);
    }
}
