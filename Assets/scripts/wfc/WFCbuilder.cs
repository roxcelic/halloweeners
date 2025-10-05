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

    [Header("data")]
    public roomData.map map;

    void Start() {
        map = new roomData.map(gridHeight, gridWidth, startRoom);

        while (map.play(possibleRooms, emptyRoom)) Debug.Log("looping...");
    }

    void Update() {
        if (eevee.input.Grab("Attack")) {
            map = new roomData.map(gridHeight, gridWidth, startRoom);

            while (map.play(possibleRooms, emptyRoom)) Debug.Log("looping...");
        }
    }

    
}
