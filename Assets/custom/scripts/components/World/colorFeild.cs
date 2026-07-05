using UnityEngine;

public class colorFeild : MonoBehaviour {
    public Color chosenColor = Color.red;

    #region colliderShit
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != "PlayerB") return;
        colorManager.data.forceColor(chosenColor);
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag != "PlayerB") return;
        colorManager.data.freeColor();
    }
    #endregion
}