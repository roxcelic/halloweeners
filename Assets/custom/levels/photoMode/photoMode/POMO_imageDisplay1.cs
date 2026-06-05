using UnityEngine;

public class POMO_imageDisplay1 : MonoBehaviour {
    private SpriteRenderer sr;

    void Start() {
        sr = transform.GetComponent<SpriteRenderer>();
    }

    void Update() {
        sr.sprite = POMO_Cont.self.imageToDisplay;
    }
}