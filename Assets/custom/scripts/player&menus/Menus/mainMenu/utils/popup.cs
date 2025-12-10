using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class popup : MonoBehaviour {
    [Header("comp")]
    public mainMenuController MMC;
    public Image display;
    public TMP_Text description;

    [Header("data")]
    public System.Action onComplete;
    public System.Action onDeny;

    void Update() {
        if (eevee.input.Collect("interact")) onComplete();
        if (eevee.input.Collect("back")) onDeny();
    }
}
