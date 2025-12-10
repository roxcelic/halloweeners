using Unity;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class WFC_map : MonoBehaviour {
    [Header("config")]
    public Vector2 offset = new Vector2(16, 16);

    public Transform target;

    public GameObject defaultImage;

    private Vector2 StartPos = new Vector2();
    private Vector2 currentPos;

    public Transform rotationTarget;
    public Transform followTarget;

    void Start() {StartCoroutine(follow());}

    public void genMap() {
        foreach (Transform child in target) Destroy(child.gameObject);
        StartPos = new Vector2((GS.live.state.map.width / 2) * offset.x, (GS.live.state.map.height / 2) * offset.y);
        currentPos = StartPos;

        for (int i = 0; i < GS.live.state.map.mapItems.Length; i++) {
            for (int z = 0; z < GS.live.state.map.mapItems[i].rooms.Length; z++) {
                // GameObject currentRoom = Instantiate(map.mapItems[i].rooms[z].roomPrefab, new Vector3(roomWidth * z, 0, roomHeight * i), map.mapItems[i].rooms[z].roomPrefab.transform.rotation, transform);

                Vector2 spawnPos = new Vector2(offset.x * (z), offset.y * (i)) - StartPos;

                if(GS.live.state.map.mapItems[i].rooms[z] != null) {
                    GameObject currentImage = spawnImage(GS.live.state.map.mapItems[i].rooms[z].image);
                    currentImage.transform.SetParent(target);
                    currentImage.transform.GetComponent<RectTransform>().anchoredPosition = spawnPos;
                    currentImage.transform.GetComponent<RectTransform>().localScale = new Vector3(1, -1, 1); // why do i have to do this
                }

                currentPos = new Vector2(currentPos.x, currentPos.y + offset.y);
            }
            currentPos = new Vector2(StartPos.x + offset.x, currentPos.y);
        }
    }

    #region utils
    public GameObject spawnImage(Sprite image) {
        GameObject newImage = Instantiate(defaultImage);
        newImage.transform.GetComponent<Image>().sprite = image;
        return newImage;
    }
    #endregion

    public IEnumerator follow () {
        yield return new WaitUntil(() => GS.live.state.loaded);

        genMap();

        Vector2 followTargetStart =  new Vector2(followTarget.position.x, followTarget.position.z);

        while (true) {
            Vector2 position = new Vector2(followTarget.position.x, followTarget.position.z) - followTargetStart;
            
            target.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2((position.x / 10) * offset.x, (position.y / 10) * offset.y);
            // rotationTarget.rotation = Quaternion.Euler(0, 0, followTarget.eulerAngles.y);

            yield return 0;
        }
    }
}
