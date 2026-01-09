using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using musicLib;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/music/ list queue")]
public class PM_listQueue : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        foreach (song songName in var.queue) {
            PMC.log(songName.name, "", "red", false);
        }
    }

    public override bool active() {return musicLib.var.customMusicAccess;}
}
