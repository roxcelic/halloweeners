using UnityEngine;
using UnityEngine.AI;

using System;
using System.Collections;
using System.Collections.Generic;

public class NEN_football : NEN_base {
    [Header("movement")]
    public pathFinding.grid grid;
    [Range(0.1f, 10f)] public float moveDelay = 0.25f;

    // main rubish
    protected override void Start() {base.Start(); grid.generate();}
    public override void begin() {StartCoroutine(pathFinding());}

    public IEnumerator pathFinding() {
        while (true) {
            List<Vector3> path = grid.findPath(transform, self.player.transform);
            
            if (path.Count > 0){
                transform.position = path[Math.Clamp(2, 0, path.Count - 1)];
                Debug.Log("teleport");
            }

            yield return new WaitForSeconds(moveDelay);
        }
    }
}