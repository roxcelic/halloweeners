using UnityEngine;

public class POR_BASE : MonoBehaviour {
    public Transform player;
    public Transform reciever;

    private bool playerIsOveralpping = false;

    protected virtual void Update() {
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

                OnTp();

                Debug.Log($"moved player");
            }
        }
    }

    protected virtual void OnTp() {}

    void OnTriggerEnter (Collider other) {
        if (other.tag == "Player" || other.tag == "PlayerB") playerIsOveralpping = true;
    }
}
