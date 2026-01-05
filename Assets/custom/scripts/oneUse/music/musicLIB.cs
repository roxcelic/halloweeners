using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace musicLib {
    public static class var {
        public static bool customMusicAccess = true;

        #nullable enable
        public static string? userSong;

        public static song? battleSong;
        public static song? biombSong;
        #nullable disable

        public static List<song> queue = new List<song>();
    }

    public static class live {
        public static AudioSource audio;
        public static backgroundMusic bg;

        public enum audioType {
            packaged,
            local
        }

        public static musicLibrary library;
    }

    [Serializable]
    public class song {
        public musicLib.live.audioType type;
        public AudioClip clip;
        public string path;
        public string name;

        public song(musicLib.live.audioType songType, AudioClip songClip, string filePath = "", string songName = "") {
            this.type = songType;
            this.clip = songClip;
            this.path = filePath;
            this.name = songName;
        }
    }

    public static class music {
        static music() {musicLib.live.library = Resources.Load<musicLibrary>("musicLibrary");}

        public static AudioSource load(backgroundMusic BG) {
            musicLib.live.bg = BG;
            return (musicLib.live.audio = BG.transform.GetComponent<AudioSource>());
        }

        #nullable enable
        public static song? getNextSong(bool remove = true) {
            if (var.queue.Count > 0 && var.customMusicAccess) {
                song next = var.queue[0];
                if (remove && var.queue.Count > 1) var.queue.RemoveAt(0);
                return next;
            } else if (var.battleSong != null) {
                return var.battleSong;
            } else if (var.biombSong != null) {
                return var.biombSong;
            } else {
                return live.library.defaultSong;
            }
        }
        #nullable disable

        public static void playSong(song toPlay) {live.bg.playSong(toPlay);}

        public static bool addSongToQueue(string name) {
            song foundSong = utils.compareSong(name);

            if (foundSong != null) {
                var.queue.Add(foundSong);
                return true;
            } else return false;
        }

        public static void loadSongs(List<song> localSongs) {
            live.library.localSongs = localSongs;
        }
        
        public static List<string> listSongs() {
            return new List<string>(live.library.getLibrary().Keys);
        }

        public static void skip() {
            live.bg.playSong(getNextSong());
        }
    }

    public static class utils {
        #nullable enable
        public static song? compareSong(string songName) {
            Dictionary<string, song> songs = live.library.getLibrary();

            if (songs.ContainsKey(songName)) return songs[songName];

            return null;
        }
        #nullable disable
    }
}