using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class Pickup : MonoBehaviour {
    [Header("config")]
    public string playerTag = "Player";
    public bool destroyOnCollect = false;
    public bool swapWeapon = true;
    public playerController player = null;
    public List<string> defaultAttacks = new List<string>();

    [Header("item")]
    public AT_base attack;

    [Header("componenets")]
    public SpriteRenderer SR;
    public brain brain;
    public string thought = "";

    [Header("text")]
    public sys.Text pickupMessage = new sys.Text();

    void Start() {
        LoadAttack(attack);

        brain = GetComponent<brain>();
    }

    void Update() {
        if (eevee.input.Collect("interact") && player != null) {
            AT_base tmp = player.attack;
            player.switchAttack(attack);

            if (destroyOnCollect) {
                // save attack
                save.saveData currentSave = save.getData.viewSave();
                currentSave.savedAttacks.Add(new AVdata.savedAttack(tmp));
                save.getData.save(currentSave);

                Destroy(transform.gameObject);
            }

            if (defaultAttacks.Contains(tmp.name)) {
                Destroy(transform.gameObject);
            }
            
            else if (swapWeapon) LoadAttack(tmp);
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
        Debug.Log($"loading attack {LDAattack.displayName.localise()}");
        attack = LDAattack;
        SR.sprite = attack.sprite;

        thought = pickupMessage.displayVar(new Dictionary<string, string>{
            {"attackDisplayName", attack.displayName.localise()},
            {"attackName", attack.attackData.name}
        });
        if (player != null) brain.thought = thought;
    }


}
