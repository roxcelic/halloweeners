using UnityEngine;
using UnityEngine.Audio;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

public class volumeController : MonoBehaviour {
    public static volumeController instance;
    
    // stuffs
    private AudioMixer audioMixer;
    
    private AudioMixerGroup MG_master;
    private AudioMixerGroup MG_music;
    private AudioMixerGroup MG_sfx;

    public enum volumeType {
        master,
        music,
        sfx
    }

    void Start() {
        instance = this;

        audioMixer = Resources.Load<AudioMixer>("audio/main");
    
        if (audioMixer != null) {

            MG_master = audioMixer.FindMatchingGroups("Master")[0];
            MG_music = audioMixer.FindMatchingGroups("music")[0];
            MG_sfx = audioMixer.FindMatchingGroups("sfx")[0];

            updateVolumes();
        } else {
            Debug.Log("no audio mixer found");
        }
    }

    public void updateVolumes() {
        // failsafe
        if (audioMixer == null) {
            Debug.Log("No audio mixer");
            return;
        }

        // update the volumes by grabbing the save
        fullConfig conf = getData.config();

        // master
        if (MG_master == null) Debug.Log("no master group");
        else audioMixer.SetFloat("MasterVol", conf.volume_master);

        if (MG_music == null) Debug.Log("no music group");
        else audioMixer.SetFloat("MusicVol", conf.volume_music);

        if (MG_sfx == null) Debug.Log("no sfx group");
        else audioMixer.SetFloat("SfxVol", conf.volume_sfx);

        // volume set
        Debug.Log("volume set :D");
    }
}
//josh was here hahahahahah 