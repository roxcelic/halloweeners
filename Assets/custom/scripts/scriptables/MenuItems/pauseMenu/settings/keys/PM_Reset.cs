using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/reset controls")]
public class PM_Reset : PM_Base {
    public bool sel = false;
    public float duration = 10000f;

    public async override void onLoad(pauseMenuController PMC) {
        float passedTime = 0;
        if (sel) return;
        sel = true;
        while (isSelected(PMC) && passedTime < duration) {
            await Task.Delay(50);
            passedTime += 50f;
            name.overrideName = isSelected(PMC) ?  $"{passedTime.ToString()}/{duration.ToString()}" : "";
            updateScreen(PMC);
        }

        if (passedTime >= duration) {
            eeveeLive.util.reset();
            name.overrideName = "@@@@@@";
        }

        sel = false;
    }

    public override void runOnLoad() {sel = false;}
    public override void action(pauseMenuController PMC, string input = "") {}
}
