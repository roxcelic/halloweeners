using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using musicLib;

public class backgroundMusic : MonoBehaviour {
    [Header("comp")]
    public AudioSource AS;
    
    // data
    public Coroutine songTimer;

    void Start() {
        AS = music.load(this);

        playSong(music.getNextSong());
        StartCoroutine(loadLocalAudio());
    }

    public void playSong(song toPlay) {
        AS.clip = toPlay.clip;
        AS.Play();
 
        if (songTimer != null) StopCoroutine(songTimer);
        songTimer = StartCoroutine(waitForSongToFinish(() => {
            playSong(music.getNextSong());
        }));
    }

    IEnumerator waitForSongToFinish(Action after){
        yield return new WaitWhile(()=> AS.isPlaying);
        after();
    }

    IEnumerator loadLocalAudio() {
        List<song> localSongs = new List<song>();
        string dirPath = $"{Application.persistentDataPath}/Audio/";
        Directory.CreateDirectory(dirPath);
        
        string[] files = Directory.GetFiles(dirPath);
        
        foreach (string file in files) {
            string[] splitFile = file.Split("/");
            string path = splitFile[splitFile.Length - 1];

            AudioType audioType = AudioType.UNKNOWN;
            if (path.EndsWith(".ogg")) {
                audioType = AudioType.OGGVORBIS;
            } else if (path.EndsWith(".wav")) {
                audioType = AudioType.WAV;
            } else if (path.EndsWith(".mp3")) {
                audioType = AudioType.MPEG;
            }

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file:///{file}", audioType)) {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError(www.error);
                } else {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                    if (clip != null) {
                        localSongs.Add(new song(live.audioType.local, clip, file, path));
                    }
                }
            }
        }

        Debug.Log($"found {localSongs.Count} songs");
        music.loadSongs(localSongs);
        if(localSongs.Count > 0) music.playSong(localSongs[0]);
    }
}