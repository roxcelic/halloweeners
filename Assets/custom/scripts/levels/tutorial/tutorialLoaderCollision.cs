using UnityEngine;

public class TutorialLoaderCollision : MonoBehaviour {
    private bool ran = false;
    void OnTriggerEnter (Collider other) {
        if ((other.tag == "Player" || other.tag == "PlayerB") && !ran) {
            TutorialLoader.instance.A_continue = true;
            ran = true;
        }
    }
}