using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/Load Keys")]
public class PM_LoadKeys : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        children = new List<PM_Base>();

        foreach (string key in eevee.inject.retrieve().FullConfig.Keys) {
            PM_editKey currentKey = ScriptableObject.CreateInstance("PM_editKey") as PM_editKey;
            currentKey.key = key;
            currentKey.name.overrideName = $"_{key}";

            children.Add(currentKey);
        }

        PMC.loadMenu(children);
    }
}
