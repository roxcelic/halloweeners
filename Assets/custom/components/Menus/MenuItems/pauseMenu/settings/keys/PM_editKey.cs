using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/hidden/edit Key")]
public class PM_editKey : PM_Base {
    public string key;

    public override void action(pauseMenuController PMC, string input = "") {
        children = new List<PM_Base>();
        /*
            load view inputs
        */
        PM_Key currentKey = ScriptableObject.CreateInstance("PM_Key") as PM_Key;
        currentKey.key = key;
        currentKey.name = "view";
        children.Add(currentKey);

        /*
            load evil
        */
        PM_Evil evilKey = ScriptableObject.CreateInstance("PM_Evil") as PM_Evil;
        evilKey.key = key;
        evilKey.tapCount = 1000;
        evilKey.name = "evil";
        children.Add(evilKey);

        /*
            load subtract input
        */
        PM_KeyRemove KeyRemove = ScriptableObject.CreateInstance("PM_KeyRemove") as PM_KeyRemove;
        KeyRemove.key = key;
        KeyRemove.name = "remove";
        children.Add(KeyRemove);

        /*
            load add input
        */
        PM_KeyAdd KeyAdd = ScriptableObject.CreateInstance("PM_KeyAdd") as PM_KeyAdd;
        KeyAdd.key = key;
        KeyAdd.name = "add";
        children.Add(KeyAdd);

        PMC.loadMenu(children);
    }
}
