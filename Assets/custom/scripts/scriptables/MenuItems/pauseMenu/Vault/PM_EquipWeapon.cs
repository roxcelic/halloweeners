using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class PM_EquipWeapon : PM_Base {
    public AVdata.savedAttack loadedAttack;

    public override void action(pauseMenuController PMC, string input = "") {
        children = new List<PM_Base>();

        PM_SwapWeapon swap = ScriptableObject.CreateInstance("PM_SwapWeapon") as PM_SwapWeapon;
        swap.loadedAttack = loadedAttack;
        swap.name = new sys.Text("swap");
        children.Add(swap);

        PM_ViewStats stats = ScriptableObject.CreateInstance("PM_ViewStats") as PM_ViewStats;
        stats.loadedAttack = loadedAttack;
        stats.name = new sys.Text("stats");
        children.Add(stats);

        PMC.loadMenu(children);
    }
}
