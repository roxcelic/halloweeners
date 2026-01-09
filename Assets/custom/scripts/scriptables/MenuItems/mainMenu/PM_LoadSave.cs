using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/load save")]
public class PM_LoadSave : PM_Base {
    [Header("load save")]
    public int saveNum = 0;

    public override void onLoad(pauseMenuController PMC) {
        name.overrideName = $"{saveNum}: {(save.data.getSaves().saves[saveNum].name == "" ? "//" : save.data.getSaves().saves[saveNum].name)}";
    }

    public async override void action(pauseMenuController PMC, string input = "") {
        PlayerPrefs.SetInt("saveSlot", saveNum);

        if(save.data.getSaves().saves[saveNum].name != "") base.action(PMC);
        else {
            save.saveData currentSave = save.getData.viewSave();
            currentSave.name = await PMC.getText(new sys.Text("", Resources.Load("text/MainMenu/saves/nameYourSave") as textobject).localise(), sys.var.keywords.defaultCharName);
            save.getData.save(currentSave);

            base.action(PMC);
        }
    }
}
