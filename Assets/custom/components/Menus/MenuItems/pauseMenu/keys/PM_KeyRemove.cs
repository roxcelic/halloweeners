using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using ext;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/hidden/KeyRemove")]
public class PM_KeyRemove : PM_Base {
    public string key;
    bool overried = false;

        // un-overried
    public override void onLoad(pauseMenuController PMC){
        overried = false;
    }

    public override void action(pauseMenuController PMC) {
        eevee.config newInput = eevee.inject.retrieve().FullConfig[key];

        switch (eevee.conf.autoDetect()) {
            case eevee.inputCL.controller:
                if (newInput.CONTROLLER_name.Length == 1 && !overried){
                    PMC.log("only one input registered, are you sure you want to remove it? (run again to confirm)", "system", "blue");
                    overried = true;
                    return;
                }
                
                if (overried) {
                    if (save.getData.isDev()) PMC.log($"input protection ovverided, if this results in you being softlocked delete the file here: <color=white>{eevee.var.ConfPath}</color>", "system", "blue");
                    else {
                        PMC.log($"sorry but in order to remove make it so no keys are registered you need to be have developer options enabled", "system", "blue");
                        return;
                    }
                }

                PMC.log($"removing binding: {newInput.CONTROLLER_name[0]} from {key}", "system", "blue");
                // Array.Resize(ref newInput.CONTROLLER_name, newInput.CONTROLLER_name.Length - 1);
                newInput.CONTROLLER_name = newInput.CONTROLLER_name.removeAtIndex(0);

                break;
            case eevee.inputCL.keyboard:
                if (newInput.KEYBOARD_code.Length == 1 && !overried){
                    PMC.log("only one input registered, are you sure you want to remove it? (run again to confirm)", "system", "blue");
                    overried = true;
                    return;
                }

                if (overried) {
                    if (save.getData.isDev()) PMC.log($"input protection ovverided, if this results in you being softlocked delete the file here: <color=white>{eevee.var.ConfPath}</color>", "system", "blue");
                    else {
                        PMC.log($"sorry but in order to remove make it so no keys are registered you need to be have developer options enabled", "system", "blue");
                        return;
                    }
                }

                PMC.log($"removing binding: {((KeyCode)newInput.KEYBOARD_code[0]).ToString()} from {key}", "system", "blue");
                // Array.Resize(ref newInput.KEYBOARD_code, newInput.KEYBOARD_code.Length - 1);
                newInput.KEYBOARD_code = newInput.KEYBOARD_code.removeAtIndex(0);

                break;
        }

        eevee.inject.OverWrite(newInput);
    }
}
