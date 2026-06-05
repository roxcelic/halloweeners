using UnityEngine;
using UnityEngine.UI;

public class POMO_ImageDisplay : MonoBehaviour {
    [Header("comp")]
    public Button button;
    public Image display;
    public Sprite sprite;
    public POMO_loadImages holder;

    public void Select() {
        holder.transform.gameObject.SetActive(false);
        POMO_Cont.self.imageToDisplay = sprite;
        POMO_Cont.self.imageDisplayButton.interactor.buttonText.text = "";
        POMO_Cont.self.imageDisplayButton.interactor.buttonDisplay.sprite = sprite;
        POMO_Cont.self.imageDisplayButton.interactor.buttonDisplay.transform.gameObject.SetActive(true);
    }
}