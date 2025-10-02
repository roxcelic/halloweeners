using UnityEngine;

public class Pickup : MonoBehaviour {
    [Header("config")]
    public string playerTag = "Player";
    public movement player = null;

    [Header("item")]
    public AT_base attack;

    void Update() {
        if (eevee.input.Collect("interact") && player != null) {
            AT_base tmp = player.attack;
            player.switchAttack(attack);
            attack = tmp;
        }
    }

    // update current collisions
	void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == playerTag) {
            player = col.transform.GetComponent<movement>();

            player.interactables.Add(this);
        }
	}

    void OnTriggerExit (Collider col) {
        if (col.gameObject.tag == playerTag) {
            player = null;

            player.interactables.Remove(this);
        }
	}


}
