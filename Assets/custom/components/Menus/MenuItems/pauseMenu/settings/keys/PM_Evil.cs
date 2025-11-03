using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/hidden/evil")]
public class PM_Evil : PM_Base {
    public int tapCount = 5;
    public string key;

    public override async void action(pauseMenuController PMC, string input = "") {
        PMC.log($"get ready to tap {key} {tapCount} times...", "evil", "blue");
        if (await eevee.input.multiTap(key, tapCount, 1000)) {
            PMC.log($"well done", "evil", "blue");
        } else {
            PMC.log($"you failed", "evil", "blue");
        }
    }
}
