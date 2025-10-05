using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "room", menuName = "rooms/base")]
public class room : ScriptableObject {
    [Header("data")]
    public roomData.roomType type;
    public List<roomData.roomDirections> allowedDirections;
    public GameObject roomPrefab;
}
