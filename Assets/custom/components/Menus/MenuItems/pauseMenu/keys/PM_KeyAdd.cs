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

    public override void action(pauseMenuController PMC) {
        eevee.config newInput = eevee.inject.retrieve().FullConfig[key];

        switch (eevee.conf.autoDetect()) {
            case eevee.inputCL.controller:
                PMC.log("after a delay the next controller input will be registered as a binding", "system", "blue");
                PMC.StartCoroutine(waitForGamepadInput(newInput, PMC));

                break;
            case eevee.inputCL.keyboard:
                PMC.log("after a delay the next keyboard input will be registered as a binding", "system", "blue");
                PMC.StartCoroutine(waitForKeyboardInput(newInput, PMC));

                break;
        }
    }

    /*
        grab the next keyboard input
    */
    public IEnumerator waitForKeyboardInput(eevee.config newInput, pauseMenuController PMC) {
        PMC.input.interactable = false;
        yield return new WaitForSecondsRealtime(0.25f);
        PMC.log("press now", "system", "blue");

        while (!Input.anyKeyDown) yield return null;

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKeyDown(kcode)) {
                Array.Resize(ref newInput.KEYBOARD_code, newInput.KEYBOARD_code.Length + 1);
                newInput.KEYBOARD_code[newInput.KEYBOARD_code.Length - 1] = (int)kcode;
                PMC.log($"registered {kcode.ToString()}", "system", "blue");

                break;
            }
        }

        eevee.inject.OverWrite(newInput);
        PMC.input.interactable = true;
        PMC.input.Select();
    }


    /*
        grab the next controller input
    */
    public IEnumerator waitForGamepadInput(eevee.config newInput, pauseMenuController PMC) {
        PMC.input.interactable = false;
        yield return new WaitForSecondsRealtime(0.25f);
        PMC.log("press now", "system", "blue");

        if (Gamepad.current != null) {
            yield return new WaitUntil(() => get_current_pressed_names().Count > 0);

            Array.Resize(ref newInput.CONTROLLER_name, newInput.CONTROLLER_name.Length + 1);
            newInput.CONTROLLER_name[newInput.CONTROLLER_name.Length - 1] = get_current_pressed_names()[0];
            PMC.log($"registered {get_current_pressed_names()[0]}", "system", "blue");

        }

        eevee.inject.OverWrite(newInput);
        PMC.input.interactable = true;
        PMC.input.Select();
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
