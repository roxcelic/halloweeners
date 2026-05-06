using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using save;

public class devMusicDisplay : MonoBehaviour {
    private TMP_Text display;
    
    void Start() {display = transform.GetComponent<TMP_Text>();}
    
    void Update() {
        if (display == null || !getData.isDev()) {
            if(display != null) display.text = $"{musicLib.var.currentSong.name} by {musicLib.var.currentSong.artist} -- [{limit(musicLib.music.getNextSong(false).name)}]";
            return;
        }

        display.text = $"{Mathf.Round(1.0f / Time.deltaTime)} :: {getData.viewSave().name} :: {Application.persistentDataPath}";
    }

    private string limit(string input, int max = 10) {
        if (input.Length > max) {
            int el = Mathf.Clamp(input.Length - max, 0, 3);
            return $"{input.Substring(0, max)}{String.Concat(Enumerable.Repeat(".", el))}";
        } else return input;
    }
}