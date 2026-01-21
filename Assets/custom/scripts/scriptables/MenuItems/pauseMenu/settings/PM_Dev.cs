using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/dev")]
public class PM_Dev : PM_Base {
    [Header("dev options")]
    public sys.Text selectMessage;
    public sys.Text languageWarning;

    public async override void action(pauseMenuController PMC, string input = "") {
        // save data
        save.saveData currentSave = save.getData.viewSave();
        
        if (!currentSave.dev && (await PMC.getText("pass")) != sys.var.keywords.devPass) return;

        // save attack
            currentSave.dev = !currentSave.dev;
            PMC.log(selectMessage.displayVar(new Dictionary<string, string>{
                {"dev", currentSave.dev.ToString()}
            }), sys.programNames.dev.localise(), "green");
            if(currentSave.dev) PMC.log(languageWarning.localise(), sys.programNames.dev.localise(), "green");

        save.getData.save(currentSave);

        PMC.selectedIndex = 0;
    }
}
