using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using musicLib;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/music/ list songs")]
public class PM_listSongs : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        children = new List<PM_Base>();

        foreach (string songName in music.listSongs()) {
            PM_queueSong songToQueue = ScriptableObject.CreateInstance("PM_queueSong") as PM_queueSong;
            songToQueue.selectedSong = songName;
            songToQueue.name.overrideName = songName;

            children.Add(songToQueue);
        }

        PMC.loadMenu(children);
    }
}
