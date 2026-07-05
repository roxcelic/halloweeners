using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using musicLib;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/music/open song folder")]
public class PM_openSongFolder : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        Application.OpenURL($"{Application.persistentDataPath}/Audio/");
    }
}
