using UnityEngine;
using UnityEngine.UI;

public class PMS_vaultItem : MonoBehaviour {
    public int position;
    public Image iconDisplay;
    public PMS_vaultSpawner parent;

    public void run() {
        PMS_vaultSpawner.instance.selectedItem = position;
        PMS_vaultSpawner.instance.openChildMenu();
    }

    public void display() {
        Debug.Log(parent);
        Debug.Log(parent.findAttackSprite(position));
        iconDisplay.sprite = parent.findAttackSprite(position);
    }
}