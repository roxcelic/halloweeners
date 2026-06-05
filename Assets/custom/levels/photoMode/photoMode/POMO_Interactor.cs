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
    public TMP_InputField input;

    public TMP_InputField VectorX;
    public TMP_InputField VectorY;
    public TMP_InputField VectorZ;

    public Slider slider;

    [Header("var")]
    public float height = 15f;
}