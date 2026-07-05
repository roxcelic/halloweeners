using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class roomConfig {
    public Transform obj;
    public Transform spawnPoint;

    public roomConfig(Transform obj) {
        this.obj = obj;
    }
}

/// <summery> the script which will controll the loaded room </summery>
public class roomLoader : MonoBehaviour {
    public static roomLoader instance;
    public List<roomConfig> rooms;
    
    // private data
    private int currentRoom = 0;

    /// <summery> load instance and setup room </summery>
    void Start() {
        instance = this; // setup instance
        currentRoom = 0;

        if (rooms.Count == 0) foreach (Transform child in transform) rooms.Add(new roomConfig(child));

        // set all children to inactive
        foreach(roomConfig child in rooms) child.obj.gameObject.SetActive(false);
        rooms[currentRoom].obj.gameObject.SetActive(true); // set the first room 
        if (rooms[currentRoom].spawnPoint != null) playerController.mainPlayer.transform.position = rooms[currentRoom].spawnPoint.position;
    }

    /// <summery> load the next room </summery>
    public void Next() {
        currentRoom++;

        // set all children to inactive
        foreach(roomConfig child in rooms) child.obj.gameObject.SetActive(false);

        if (currentRoom == rooms.Count) {
            Debug.Log("finished rooms");
            return;
        }

        rooms[currentRoom].obj.gameObject.SetActive(true); // set the current room 
        if (rooms[currentRoom].spawnPoint != null) playerController.mainPlayer.transform.position = rooms[currentRoom].spawnPoint.position;

        sys.utils.playScreenEffect("glitch");
    }

    /// <summery> load the previous room </summery>
    public void Last() {
        currentRoom--;
        
        // set all children to inactive
        foreach(roomConfig child in rooms) child.obj.gameObject.SetActive(false);
        
        if (currentRoom == -1) {
            Debug.Log("finished rooms :: -1");
            return;
        }

        rooms[currentRoom].obj.gameObject.SetActive(true); // set the current room 
        if (rooms[currentRoom].spawnPoint != null) playerController.mainPlayer.transform.position = rooms[currentRoom].spawnPoint.position;
        sys.utils.playScreenEffect("glitch");
    }
}