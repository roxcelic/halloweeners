using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class despawn : MonoBehaviour {
    [Range(0f, 50f)] public float lifeSpan = 10f;

    void Start() {
        StartCoroutine(dieAfterDuration());
    }

    public IEnumerator dieAfterDuration() {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(transform.gameObject);
    }
}