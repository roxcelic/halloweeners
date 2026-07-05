using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using player.utils;

using save;

public class PM_SwapWeapon : PM_Base {
    public AVdata.savedAttack loadedAttack;

    public override void action(pauseMenuController PMC, string input = "") {
        saveData currentSave = getData.viewSave();
        
        AT_base currentSelectedAttack = GS.live.state.getCurrentAttack(loadedAttack.attackName);
        currentSelectedAttack.attackData = loadedAttack.attackData;
        AT_base attack = playerController.mainPlayer.Reset();

        playerController.mainPlayer.switchAttack(currentSelectedAttack);

        Int32 attackIndex = currentSave.savedAttacks.IndexOf(loadedAttack);

        if (attack != null) currentSave.savedAttacks[attackIndex] = new AVdata.savedAttack(attack);
        else currentSave.savedAttacks.RemoveAt(attackIndex);

        getData.save(currentSave);

        PMC.reset();
    }
}
