using UnityEngine;

public class colorFeild : MonoBehaviour {
    public Color chosenColor = Color.red;
    public string playerTag = "Player";

    #region colliderShit
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != playerTag) return;
        colorManager.data.forceColor(chosenColor);
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag != playerTag) return;
        colorManager.data.freeColor();
    }
    #endregion
}