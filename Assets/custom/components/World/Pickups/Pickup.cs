using UnityEngine;

public class Pickup : MonoBehaviour {
    [Header("config")]
    public string playerTag = "Player";
    public playerController player = null;

    [Header("item")]
    public AT_base attack;

    [Header("componenets")]
    public SpriteRenderer SR;
    public brain brain;
    public string thought = "";

    void Start() {
        LoadAttack(attack);

        brain = GetComponent<brain>();
    }

    void Update() {
        if (eevee.input.Collect("interact") && player != null) {
            AT_base tmp = player.attack;
            player.switchAttack(attack);

            LoadAttack(tmp);
        }
    }

    // update current collisions
	void OnTriggerEnter (Collider col) {
        if (col.gameObject.tag == playerTag) {
            player = col.transform.GetComponent<playerController>();

            // player.interactables.Add(this);
            brain.thought = thought;
        }
	}

    void OnTriggerExit (Collider col) {
        if (col.gameObject.tag == playerTag) {
            player = null;

            // player.interactables.Remove(this);
            brain.thought = "";
        }
	}

    void LoadAttack(AT_base LDAattack) {
        Debug.Log($"loading attack {LDAattack.name}");
        attack = LDAattack;
        SR.sprite = attack.sprite;

        thought = sys.text.displayKeyButton($"press !key:interact to pickup {attack.name}");
        if (player != null) brain.thought = thought;
    }


}
