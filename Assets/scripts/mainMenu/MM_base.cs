using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/base")]
public class MM_base : ScriptableObject {
    public string display;
    public List<MM_base> children;

    public virtual void action(mainMenuController MMC) {
        MMC.loadNextMenu(children);
    }
}
