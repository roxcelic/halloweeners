using UnityEngine;

public class FPD_ToCurtain : MonoBehaviour {
    public Material Mat;
    void Update() {
        Mat.SetVector("_playerPos", playerController.mainPlayer.transform.position);
    }
}
