using UnityEngine;
using player.utils;

[RequireComponent(typeof(brain))]
public class Pickup_Activator : MonoBehaviour {
    public AT_base attack;
    public sys.Text pickupText = new sys.Text($"press key:interact to pick up");
    public GameObject onPickupActivate;
    public bool deactivateOnCollect = false;

    void Start() {
        if (attack == null) return;

        // get the brain component
        brain self = transform.GetComponent<brain>();    
    
        self.thought = $"{pickupText.localise()} {attack.displayName.localise()}";

        self.onInteract = (playerController player) => {
            if (deactivateOnCollect) transform.gameObject.SetActive(false);
            player.switchAttack(attack);

            if (onPickupActivate != null) onPickupActivate.SetActive(true);
        };
    }
}
