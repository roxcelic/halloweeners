using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/hidden/KeyAdd")]
public class PM_KeyAdd : PM_Base {
    public string key;

    [Header("text")]
    public sys.Text delayMessage = new sys.Text();
    public sys.Text pressMessage = new sys.Text();
    public sys.Text registeredMessage = new sys.Text();

    public override void onLoad(pauseMenuController PMC) {
        delayMessage.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/add/delay") as textobject;
        pressMessage.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/add/press") as textobject;
        registeredMessage.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/add/registered") as textobject;
    }

    public override void action(pauseMenuController PMC, string input = "") {
        eevee.config newInput = eevee.inject.retrieve().FullConfig[key];

        switch (eevee.conf.autoDetect()) {
            case eevee.inputCL.controller:
                PMC.log(delayMessage.localise(), sys.programNames.system.localise(), "blue");
                PMC.StartCoroutine(waitForGamepadInput(newInput, PMC));

                break;
            case eevee.inputCL.keyboard:
                PMC.log(delayMessage.localise(), sys.programNames.system.localise(), "blue");
                PMC.StartCoroutine(waitForKeyboardInput(newInput, PMC));

                break;
        }
    }

    /*
        grab the next keyboard input
    */
    public IEnumerator waitForKeyboardInput(eevee.config newInput, pauseMenuController PMC) {
        PMC.interactable = true;
        yield return new WaitForSecondsRealtime(0.25f);
        PMC.log(pressMessage.localise(), sys.programNames.system.localise(), "blue");

        while (!Input.anyKeyDown) yield return null;

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                Array.Resize(ref newInput.KEYBOARD_code, newInput.KEYBOARD_code.Length + 1);
                newInput.KEYBOARD_code[newInput.KEYBOARD_code.Length - 1] = (int)kcode;
                PMC.log(registeredMessage.displayVar(new Dictionary<string, string>{
                    {"key", kcode.ToString()}
                }), sys.programNames.system.localise(), "blue");

                break;
            }
        }

        eevee.inject.OverWrite(newInput);
        PMC.interactable = true;
    }


    /*
        grab the next controller input
    */
    public IEnumerator waitForGamepadInput(eevee.config newInput, pauseMenuController PMC) {
        PMC.interactable = false;
        yield return new WaitForSecondsRealtime(0.25f);
        PMC.log(pressMessage.localise(), sys.programNames.system.localise(), "blue");

        if (Gamepad.current != null) {
            yield return new WaitUntil(() => get_current_pressed_names().Count > 0);

            Array.Resize(ref newInput.CONTROLLER_name, newInput.CONTROLLER_name.Length + 1);
            newInput.CONTROLLER_name[newInput.CONTROLLER_name.Length - 1] = get_current_pressed_names()[0];
            PMC.log(registeredMessage.displayVar(new Dictionary<string, string>{
                {"key", get_current_pressed_names()[0]}
            }), sys.programNames.system.localise(), "blue");

        }

        eevee.inject.OverWrite(newInput);
        PMC.interactable = true;
    }

    /*
        a util to get the current controller inputs
    */
    public List<string> get_current_pressed_names() {
        List<ButtonControl> current_pressed_buttons = Gamepad.current.allControls.OfType<ButtonControl>()
            .Where(control => control.isPressed)
            .ToList();

        List<string> current_pressed_names = current_pressed_buttons.ConvertAll<string>(control => control.displayName);

        return current_pressed_names;
    }
}
