using UnityEngine;

public class POR_BASE : MonoBehaviour {
    public Transform player;
    public Transform reciever;

    private bool playerIsOveralpping = false;

    void Update() {
        if (playerIsOveralpping) {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            if (dotProduct < -0f) {
                float rotationDIff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDIff += 180;
                player.Rotate(Vector3.up, rotationDIff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDIff, 0f) * portalToPlayer;
                player.position = reciever.position + positionOffset;
                playerIsOveralpping = false;

                Debug.Log($"moved player");
            }
        }
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("overlap");
        if (other.tag == "Player") playerIsOveralpping = true;
    }
}
