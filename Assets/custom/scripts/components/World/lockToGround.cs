using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockToGround : MonoBehaviour {
    public float groundCheckDistance = 15f;

    void Update() {placeOnGround();}

    public void placeOnGround() {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, groundCheckDistance);
        hits =  hits.Where(subject => subject.transform.gameObject.tag != "Enemy").ToArray();

        foreach(RaycastHit hit in hits) {
            if (hit.transform.gameObject.layer == sys.var.layers.ground || hit.transform.gameObject.layer == sys.var.layers.altGround) transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red; 

        Gizmos.DrawRay(transform.position, transform.forward * groundCheckDistance);
    }
}
