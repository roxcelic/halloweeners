using UnityEngine;
using UnityEngine.UI;

public class POMO_ImageDisplay : MonoBehaviour {
    [Header("comp")]
    public Button button;
    public Image display;
    public Sprite sprite;

    public void Select() {
        if(POMO_ImageDisplay_Cont.self.loadedAction != null) POMO_ImageDisplay_Cont.self.loadedAction(sprite);
        POMO_ImageDisplay_Cont.self.enable(false);
    }
}