using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/hidden/Key")]
public class PM_Key : PM_Base {
    public string key;

    public override void action(pauseMenuController PMC, string input = "") {
        // eevee.inject.retrieve().FullConfig.Keys[key]
        switch (eevee.conf.autoDetect()) {
            case eevee.inputCL.controller:
                foreach (string buttonName in eevee.inject.retrieve().FullConfig[key].CONTROLLER_name) PMC.log(buttonName, sys.programNames.controller_input.localise(), "white");

                break;
            case eevee.inputCL.keyboard:
                foreach (int keyCode in eevee.inject.retrieve().FullConfig[key].KEYBOARD_code) PMC.log(((KeyCode)keyCode).ToString(), sys.programNames.keyboard_input.localise(), "white");

                break;
        }
    }
}
