using UnityEngine;

public class colorFeild : MonoBehaviour {
    public Color chosenColor = Color.red;

    #region colliderShit
    void OnTriggerEnter(Collider other) {
        if (other.gameObject != playerController.mainPlayer.transform.gameObject) return;
        colorManager.data.forceColor(chosenColor);
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject != playerController.mainPlayer.transform.gameObject) return;
        colorManager.data.freeColor();
    }
    #endregion
}