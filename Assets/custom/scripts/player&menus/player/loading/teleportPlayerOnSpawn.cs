using UnityEngine;

public class teleportPlayerOnSpawn : MonoBehaviour {
    void Start() {
        GameObject.FindGameObjectsWithTag("Player")[0].transform.position = transform.position;
    }

}