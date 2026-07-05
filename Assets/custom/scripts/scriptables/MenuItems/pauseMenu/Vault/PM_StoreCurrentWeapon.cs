using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using player.utils;

using save;

public class PM_StoreCurrentWeapon : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        saveData currentSave = getData.viewSave();
        
        AT_base attack = playerController.mainPlayer.Reset();

        playerController.mainPlayer.Reset();

        currentSave.savedAttacks.Add(new AVdata.savedAttack(attack));

        currentSave.currentAttack = "";
        currentSave.currentAbility = "";
        currentSave.currentAttackData = new attack.attackData();

        getData.save(currentSave);

        PMC.reset();
    }

    public override bool active() {
        return playerController.mainPlayer.attack != null;
    }
}
