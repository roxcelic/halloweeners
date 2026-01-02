using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using musicLib;

[CreateAssetMenu(fileName = "musicLibrary", menuName = "musicLibrary")]
public class musicLibrary : ScriptableObject {
    public List<song> songs;
    public List<song> localSongs;
    public song defaultSong;

    public Dictionary<string, song> getLibrary() {
        Dictionary<string, song> allSongs = new Dictionary<string, song>();

        foreach (song clip in songs) allSongs.Add(clip.name, clip);
        foreach (song clip in localSongs) allSongs.Add(clip.name, clip);

        return allSongs;
    }
}
