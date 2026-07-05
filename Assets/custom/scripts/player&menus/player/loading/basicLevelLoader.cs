using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class basicLevelLoader : MonoBehaviour {
    // basic var
    private LoadingScreen player;
    public bool customMusic = true;
    public bool freeColor = true;

    void Start() {
        StartCoroutine(waitForPlayer());
    }

    public IEnumerator waitForPlayer() {
        yield return new WaitUntil(() => playerController.mainPlayer != null);

        player = playerController.mainPlayer.transform.GetComponent<LoadingScreen>();        
        if (player != null) player.completion = 100;
        musicLib.var.customMusicAccess = customMusic;    
        colorManager.data.useChosenColor = freeColor;
    }
}
