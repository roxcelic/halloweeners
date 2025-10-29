using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/base")]
public class PM_Base : ScriptableObject {
    public string name;
    public bool essential;
    public List<PM_Base> children;

    public virtual void action(pauseMenuController PMC) {
        PMC.loadMenu(children);
    }
}
