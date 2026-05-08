using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class roomMaker : basic {
    public room rooms = null;
    public int floor = 0;

     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
     public void ahhhh() {
        if (rooms != null) roomCollection.data.rooms[floor].Add(
            rooms
        );
        Debug.Log($"added: {rooms}");
     }
}