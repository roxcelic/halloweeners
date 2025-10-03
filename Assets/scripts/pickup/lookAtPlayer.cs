using UnityEngine;

public class lookAtPlayer : MonoBehaviour {
    public Transform player;
    public string playerTag = "Player";
    public bool findPlayer = true;

    [Header("locks")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    void Start() {
        if (findPlayer) player = GameObject.FindGameObjectsWithTag(playerTag)[0].transform;
    }

    void Update() {
        if (player != null) {
            Vector3 targetPos = new Vector3(lockX ? transform.position.x : player.position.x, lockY ? transform.position.y : player.position.y, lockZ ? transform.position.z : player.position.z);
            transform.LookAt(targetPos, Vector3.up);
        }
    }
}
