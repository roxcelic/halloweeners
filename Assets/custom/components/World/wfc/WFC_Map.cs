using Unity;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class WFC_map : MonoBehaviour {
    [Header("config")]
    public Vector3 StartPos = new Vector3();
    public Vector2 offset = new Vector2(16, 16);

    public Transform target;

    public GameObject defaultImage;

    private Vector3 currentPos;

    void OnEnable(){
        foreach (Transform child in target) Destroy(child.gameObject);
        currentPos = StartPos;

        for (int i = 0; i < GS.live.state.map.mapItems.Length; i++) {
            for (int z = 0; z < GS.live.state.map.mapItems[i].rooms.Length; z++) {

                if(GS.live.state.map.mapItems[i].rooms[z] != null) spawnImage(GS.live.state.map.mapItems[i].rooms[z].image, currentPos).transform.SetParent(target);
                currentPos = new Vector3(currentPos.x + offset.x, currentPos.y, currentPos.z);
            }
            currentPos = new Vector3(StartPos.x, currentPos.y + offset.y, currentPos.z);
        }
    }

    #region utils
    public GameObject spawnImage(Sprite image, Vector3 location) {
        GameObject newImage = Instantiate(defaultImage);
        newImage.transform.GetComponent<RectTransform>().anchoredPosition = location;
        newImage.transform.GetComponent<Image>().sprite = image;
        return newImage;
    }
    #endregion
}
