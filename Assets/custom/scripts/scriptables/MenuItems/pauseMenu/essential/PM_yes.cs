using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class PM_yes : PM_Base {
    public System.Action followUp;

    public override void onLoad(pauseMenuController PMC) {
        name = new sys.Text("", Resources.Load<textobject>("text/sys/yes") as textobject);
    }


    public override void action(pauseMenuController PMC, string input = "") {
        followUp();
    }
}
