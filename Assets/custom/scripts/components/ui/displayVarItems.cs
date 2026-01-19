using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class displayVarItems : MonoBehaviour {
    [Header("amount")]
    [Range(0, 10)] public int horizontal = 5;
    [Range(0, 10)] public int verticle = 5;

    public int width = 100;
    public int height = 100;

    [Header("comp")]
    public GameObject toSpawn;

    [Header("config")]
    public bool matchSize = true;
    public Vector2 offset = new Vector2();

    /// <summery> spawn all children </summery>
    void OnEnable() {
        if (matchSize) {
            RectTransform selfRect = transform.GetComponent<RectTransform>();
            
            width = (int)selfRect.sizeDelta.x;
            height = (int)selfRect.sizeDelta.y;
        }

        StartCoroutine(spawnChildren());
    }

    /// <summery> destroy all kids </summery>
    void OnDisable() {
        foreach(Transform child in transform) Destroy(child.gameObject);
    }

    /// <summery> an overrideable void for custom components </summery>
    public virtual GameObject editSpawn(GameObject spawned) {
        return spawned;
    }

    /// <summery> animate spawn </summery>
    public IEnumerator spawnChildren() {
        Vector2 currentPos = new Vector2();

        int tmpX = 0;
        int tmpY = 0;

        while (verticle > tmpY && transform.gameObject.activeSelf) {
            while(horizontal > tmpX) {
                currentPos = new Vector2(
                    ((width / horizontal) * tmpX) - (width / 2),
                    (height - ((height / verticle) * tmpY)) - (height / 2)
                ) + offset;

                Debug.Log(currentPos);

                RectTransform trans = editSpawn(Instantiate(toSpawn, new Vector3(), Quaternion.identity)).transform.GetComponent<RectTransform>();
                trans.SetParent(transform);
                trans.localPosition = currentPos;

                tmpX++;

                yield return new WaitForSecondsRealtime(0.05f);
            }

            tmpX = 0;
            tmpY++;
        }
    }
}