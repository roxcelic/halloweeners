using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class POMO_Interactor : MonoBehaviour {
    [Header("global comp")]
    public TMP_Text text;

    [Header("local comp")]
    public Button button;
    public TMP_Text buttonText;
    public Image buttonDisplay;

    public Slider slider;

    [Header("var")]
    public float height = 15f;
}