using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/change language")]
public class PM_changeLanguage : PM_Base {

    public async override void action(pauseMenuController PMC, string input = "") {
        
        PMC.loadMenu(
            generateChildren<PM_StringInput>(sys.var.keywords.languages, (String input)  => {
                Debug.Log(input);

                fullConfig conf = getData.config();
                conf.language = input; 
                getData.saveConfig(conf);

                PMC.loadPrevMenu();
            })
        );

    }
}
