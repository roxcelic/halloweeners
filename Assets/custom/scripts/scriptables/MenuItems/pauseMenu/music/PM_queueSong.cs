using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using musicLib;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/music/queue songs")]
public class PM_queueSong : PM_Base {
    public string selectedSong;

    public override void action(pauseMenuController PMC, string input = "") {
    music.addSongToQueue(selectedSong);
        PMC.log($"added {selectedSong} to queue");
        PMC.loadPrevMenu();
    }

    public override bool active() {
        return musicLib.var.customMusicAccess;
    }
}
