using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/base")]
public class MM_base : ScriptableObject {
    public string display;
    public List<MM_base> children;

    public virtual void action(mainMenuController MMC = null, pauseMenuController PMC = null) {
        if (MMC != null) {
            MMC.loadNextMenu(children);
        }
    }
}
