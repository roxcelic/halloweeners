using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class M_waterControl : MonoBehaviour {
    Material waterMat;
    public Vector3 set;

    // update the block
    public bool active = true;

    public float width;
    public float height;

    void Start() {
        // get the material and create an instance of it
        waterMat =  new Material(transform.GetComponent<Renderer>().material);
        transform.GetComponent<Renderer>().material = waterMat;

        // get sizes
        Collider m_Collider = transform.GetComponent<Collider>();

        width = m_Collider.bounds.size.x;
        height = m_Collider.bounds.size.z;

        // start loop
        StartCoroutine(foreverloop());
    }

    void OnCollisionStay(Collision collisionInfo) {
        float x = collisionInfo.contacts[0].point.x + (width / 2);
        float z = collisionInfo.contacts[0].point.z + (height / 2);

        Debug.Log($"{x} is {(x / width) * 100}% of {width} :: {x / width}");
        Vector3 bleh = new Vector3(
            Mathf.Clamp((x / width) - 0.5f, -0.3f, 0.3f),
            Mathf.Clamp((z / height) - 0.5f, -0.3f, 0.3f),
            0
        );

        set = bleh;
    }

    public IEnumerator foreverloop() {
        while (true) {
            if (active) waterMat.SetVector("_rippleOrigin", set);
            yield return new WaitForSeconds(0.25f);
        }
    }
}