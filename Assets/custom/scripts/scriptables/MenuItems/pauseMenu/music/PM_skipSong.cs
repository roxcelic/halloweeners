using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using musicLib;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/music/ skip songs")]
public class PM_skipSong : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        music.skip();
    }
}
