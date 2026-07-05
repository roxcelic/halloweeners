using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/vault/loadWeapons")]
public class PM_LoadWeapons : PM_Base {
    public override void onLoad(pauseMenuController PMC) {}

    public override void action(pauseMenuController PMC, string input = "") {
        children = new List<PM_Base>();
        saveData currentSave = getData.viewSave();

        foreach (AVdata.savedAttack att in currentSave.savedAttacks) {
            PM_EquipWeapon foundWeapon = ScriptableObject.CreateInstance("PM_EquipWeapon") as PM_EquipWeapon;
            foundWeapon.loadedAttack = att;
            foundWeapon.name = GS.live.state.getCurrentAttack(att.attackName).displayName;

            children.Add(foundWeapon);
        }

        PM_StoreCurrentWeapon storeButton = ScriptableObject.CreateInstance("PM_StoreCurrentWeapon") as PM_StoreCurrentWeapon;
        storeButton.name = new sys.Text("store");
        children.Add(storeButton);

        PMC.loadMenu(children);
    }
}
