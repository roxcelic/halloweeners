using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/open sub menu")]
public class PM_openSubMenu : PM_Base {
    public enum E_menuType {
        colorPicker,
        vault
    }

    public E_menuType menuType = E_menuType.vault;

    public override void action(pauseMenuController PMC, string input = "") {
        if (PMC.colorPicker.activeSelf || PMC.vault.activeSelf) {
            PMC.log("a sub menu is already open", "system", "red");
            return;
        }
        
        switch(menuType) {
            case E_menuType.colorPicker:
                PMC.colorPicker.SetActive(true);
                break;
            case E_menuType.vault:
                PMC.vault.SetActive(true);
                break;
        }
    }
}
