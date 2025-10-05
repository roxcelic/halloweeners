using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class WFCbuilder : MonoBehaviour {
    [Header("config")]
    public room startRoom;
    public room emptyRoom;

    public int gridWidth = 10;
    public int gridHeight = 10;

    public int roomHeight = 1;
    public int roomWidth = 1;

    public List<room> possibleRooms;

    [Range(0f, 100f)] public float perfection = 0;

    [Header("data")]
    public roomData.map map;

    void Start() {
        map = new roomData.map(gridHeight, gridWidth, startRoom);

        StartCoroutine(buildWord());
    }


    public IEnumerator buildWord() {
        Debug.Log("generating map");
        
        int count = 0;
        while (map.play(possibleRooms, emptyRoom)) {
            Debug.Log($"itteration {count}");
            count++;
            yield return 0;
        }

        yield return 0;
        Debug.Log("finished generating map, moving on to building the map");

        count = 0;
        for (int i = 0; i < map.mapItems.Length; i++) {
            for (int z = 0; z < map.mapItems[i].rooms.Length; z++) {
                if (map.mapItems[i].rooms[z] != null && map.mapItems[i].rooms[z] != emptyRoom && map.mapItems[i].rooms[z].roomPrefab != null) {
                    Instantiate(map.mapItems[i].rooms[z].roomPrefab, new Vector3(roomWidth * z, 0, roomHeight * i), map.mapItems[i].rooms[z].roomPrefab.transform.rotation);
                    yield return 0;
                }
                
                perfection = ((float)count / ((float)gridWidth * (float)gridHeight)) * 100;
                count++;
            }
        }

        Debug.Log("finished building the map");
    }
}
