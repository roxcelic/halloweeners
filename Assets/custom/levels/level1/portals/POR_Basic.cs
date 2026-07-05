using UnityEngine;

public class POR_Basic : MonoBehaviour {
    public Transform target;
    public bool nextRoom = false;
    public bool screenEffect = true;

    void OnTriggerEnter (Collider other) {
        if ((other.tag == "Player" || other.tag == "PlayerB") && target != null) {
            sys.var.components.player().transform.position = target.position;

            if (screenEffect) {
                sys.utils.playScreenEffect("glitch");
            }

            if (nextRoom) {
                roomLoader.instance.Next();
            }
        }
    }
}
