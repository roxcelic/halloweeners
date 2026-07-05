using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/open save")]
public class PM_OpenSave : PM_Base {
    [Header("open save")]
    public List<PM_Base> playOption;

    public override void onLoad(pauseMenuController PMC) {save.data.makeSaves(3);}

    public override void action(pauseMenuController PMC, string input = "") {
        int saveCount = 0;
        
        children = new List<PM_Base>();

        foreach (save.saveData save in save.data.getSaves().saves) {
            PM_LoadSave currentSave = ScriptableObject.CreateInstance("PM_LoadSave") as PM_LoadSave;
            currentSave.saveNum = saveCount;
            currentSave.children = playOption;

            children.Add(currentSave);
            saveCount++;
        }

        base.action(PMC);
    }
}
