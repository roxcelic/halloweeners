using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PM_no : PM_Base {
    public override void onLoad(pauseMenuController PMC) {
        name = new sys.Text("", Resources.Load<textobject>("text/sys/no") as textobject);
    }

    public override void action(pauseMenuController PMC, string input = "") {
        PMC.loadPrevMenu();
    }
}
