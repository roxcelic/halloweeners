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

    [Header("text")]
    public sys.Text singularInputWarning = new sys.Text();
    public sys.Text inputProtectionOveride = new sys.Text();
    public sys.Text inputProtectionOverideError = new sys.Text();
    public sys.Text removeBinding = new sys.Text();

        // un-overried
    public override void onLoad(pauseMenuController PMC){
        overried = false;

        singularInputWarning.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/remove/singularInputWarning") as textobject;
        inputProtectionOveride.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/remove/inputProtectionOveride") as textobject;
        inputProtectionOverideError.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/remove/inputProtectionOverideError") as textobject;
        removeBinding.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/remove/removeBinding") as textobject;
    }

    public override void action(pauseMenuController PMC, string input = "") {
        eevee.config newInput = eevee.inject.retrieve().FullConfig[key];

        switch (eevee.conf.autoDetect()) {
            case eevee.inputCL.controller:
                if (newInput.CONTROLLER_name.Length == 1 && !overried){
                    PMC.log(singularInputWarning.localise(), sys.programNames.system.localise(), "blue");
                    overried = true;
                    return;
                }
                
                if (overried) {
                    if (save.getData.isDev()) PMC.log(inputProtectionOveride.displayVar(new Dictionary<string, string>{{"conf", eevee.var.ConfPath}}), sys.programNames.system.localise(), "blue");
                    else {
                        PMC.log(inputProtectionOverideError.localise(), sys.programNames.system.localise(), "blue");
                        return;
                    }
                }

                PMC.log(removeBinding.displayVar(new Dictionary<string, string>{{"binding", newInput.CONTROLLER_name[0]}, {"key", key}}), sys.programNames.system.localise(), "blue");
                // Array.Resize(ref newInput.CONTROLLER_name, newInput.CONTROLLER_name.Length - 1);
                newInput.CONTROLLER_name = newInput.CONTROLLER_name.removeAtIndex(0);

                break;
            case eevee.inputCL.keyboard:
                if (newInput.KEYBOARD_code.Length == 1 && !overried){
                    PMC.log(singularInputWarning.localise(), sys.programNames.system.localise(), "blue");
                    overried = true;
                    return;
                }

                if (overried) {
                    if (save.getData.isDev()) PMC.log(inputProtectionOveride.displayVar(new Dictionary<string, string>{{"conf", eevee.var.ConfPath}}), sys.programNames.system.localise(), "blue");
                    else {
                        PMC.log(inputProtectionOverideError.localise(), sys.programNames.system.localise(), "blue");
                        return;
                    }
                }

                PMC.log(removeBinding.displayVar(new Dictionary<string, string>{{"binding", ((KeyCode)newInput.KEYBOARD_code[0]).ToString()}, {"key", key}}), sys.programNames.system.localise(), "blue");

                // Array.Resize(ref newInput.KEYBOARD_code, newInput.KEYBOARD_code.Length - 1);
                newInput.KEYBOARD_code = newInput.KEYBOARD_code.removeAtIndex(0);

                break;
        }

        eevee.inject.OverWrite(newInput);
    }
}
