using UnityEngine;
using player.utils;
using save;

[RequireComponent(typeof(brain))]
public class Pickup_Activator : MonoBehaviour {
    public AT_base attack;
    public sys.Text pickupText = new sys.Text($"press key:interact to pick up");
    public GameObject onPickupActivate;
    public bool deactivateOnCollect = false;
    public bool destroyOnCollect = false;

    void Start() {
        if (attack == null) return;

        // get the brain component
        brain self = transform.GetComponent<brain>();    
    
        self.thought = $"{pickupText.localise()} {attack.displayName.localise()}";

        self.onInteract = (playerController player) => {
            if (deactivateOnCollect) transform.gameObject.SetActive(false);
            if (destroyOnCollect) Destroy(transform.gameObject);

            AT_base storedAttack = player.Reset();
            player.switchAttack(attack);

            if (storedAttack != null) {
                saveData currentSave = getData.viewSave();
                currentSave.savedAttacks.Add(new AVdata.savedAttack(storedAttack));
                getData.save(currentSave);
            }

            if (onPickupActivate != null) onPickupActivate.SetActive(true);
        };
    }
}
