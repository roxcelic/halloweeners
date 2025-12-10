using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/hidden/evil")]
public class PM_Evil : PM_Base {
    public int tapCount = 5;
    public string key;

    [Header("text")]
    public sys.Text startMessage = new sys.Text();
    public sys.Text winMessage = new sys.Text();
    public sys.Text failMessage = new sys.Text();

    public override void onLoad(pauseMenuController PMC) {
        startMessage.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/evil/start") as textobject;
        winMessage.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/evil/win") as textobject;
        failMessage.text = Resources.Load("text/player ui/pauseMenu/commands/settings/keys/evil/fail") as textobject;
    }

    public override async void action(pauseMenuController PMC, string input = "") {
        PMC.log(startMessage.displayVar(new Dictionary<string, string>{
                {"key", key},
                {"tapCount", tapCount.ToString()},
        }), sys.programNames.evil.localise(), "blue");

        if (await eevee.input.multiTap(key, tapCount, 1000)) {
            PMC.log(winMessage.localise(), sys.programNames.evil.localise(), "blue");
        } else {
            PMC.log(failMessage.localise(), sys.programNames.evil.localise(), "blue");
        }
    }
}
