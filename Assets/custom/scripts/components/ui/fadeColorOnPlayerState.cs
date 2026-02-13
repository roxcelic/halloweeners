using UnityEngine;
using UnityEngine.UI;

public class fadeColorOnPlayerState : MonoBehaviour {
    [Header("colors")]
    public Color baseColor;
    public Color targetColor;

    [Header("data")]
    public playerState state;
    public enum playerState {
        canDash,
        canJump
    }

    private Image img;

    void Start() {
        img = transform.GetComponent<Image>();
    }

    void Update() {
        switch (state) {
            case playerState.canDash:
                img.color = Color.Lerp(img.color, playerController.mainPlayer.canDash ? targetColor : baseColor, Time.deltaTime * 5f);

                break;
            case playerState.canJump:
                img.color = Color.Lerp(img.color, playerController.mainPlayer.jumpCount > 0 ? targetColor : baseColor, Time.deltaTime * 5f);

                break;
        }
    }
}