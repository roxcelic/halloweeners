using UnityEngine;
using UnityEngine.UI;

public class PMS_vaultItem : MonoBehaviour {
    public int position;
    public Image iconDisplay;

    public void run() {
        PMS_vaultSpawner.instance.selectedItem = position;
        PMS_vaultSpawner.instance.openChildMenu();
    }

    public void display() {
        iconDisplay.sprite = PMS_vaultSpawner.findAttackSprite(position);
    }
}