using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class growColliderWhileOut : MonoBehaviour {
    [Range(0.1f, 10f)] public float growthRate = 1f;
    private BoxCollider col;

    public void Start() {
        col = transform.GetComponent<BoxCollider>();
        if (col != null) StartCoroutine(grow());
    }

    public IEnumerator grow() {
        float elapsedTime = 0f;
        Vector3 originalSize = col.size;

        while(true) {
            elapsedTime += Time.deltaTime;
            
            col.size = originalSize + (originalSize* elapsedTime * growthRate);

            yield return 0;
        }
    }
}