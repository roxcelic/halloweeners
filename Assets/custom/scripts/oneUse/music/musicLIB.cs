using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace musicLib {
    public static class var {
        public static bool customMusicAccess = false;

        public static string? userSong;

        public static song? battleSong;
        public static song? biombSong;

        public static Dictionary<string, song> queue = new Dictionary<string, song>();
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

        public static song? getNextSong(bool remove = true) {
            if (var.queue.Keys.Count > 0 && var.customMusicAccess) {
                song next = var.queue[new List<string>(var.queue.Keys)[0]];
                if (remove && var.queue.Keys.Count > 1) var.queue.Remove(new List<string>(var.queue.Keys)[0]);
                return next;
            } else if (var.battleSong != null) {
                return var.battleSong;
            } else if (var.biombSong != null) {
                return var.biombSong;
            } else {
                return live.library.defaultSong;
            }
        }

        public static void playSong(song toPlay) {live.bg.playSong(toPlay);}

        public static bool addSongToQueue(string name) {
            song foundSong = utils.compareSong(name);

            if (foundSong != null) {
                var.queue.Add(name, foundSong);
                return true;
            } else return false;
        }

        public static void loadSongs(List<song> localSongs) {
            live.library.localSongs = localSongs;
        }
    }

    public static class utils {
        public static song? compareSong(string songName) {
            Dictionary<string, song> songs = live.library.getLibrary();

            if (songs.ContainsKey(songName)) return songs[songName];

            return null;
        }
    }
}