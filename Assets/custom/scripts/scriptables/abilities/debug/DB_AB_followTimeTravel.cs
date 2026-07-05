using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using ext;

public class DB_AB_followTimeTravel : MonoBehaviour {
    public AB_timeTravel target;
    [Range(0, 15f)] public float travelDistance = 1f;
    
    void Update() {
        float selectedTime = Mathf.Clamp(travelDistance, 0, target.distance);
        float mod = selectedTime;

        Debug.Log($"inputted time {Time.time - selectedTime}");

        selectedTime = target.TimeDevice.Keys.ToList().FindClosestIndex(Time.time - selectedTime);

        Debug.Log($"matched time was {selectedTime}");
        if(target.TimeDevice.ContainsKey(selectedTime)) transform.position = target.TimeDevice[selectedTime].pos;
    }
}
