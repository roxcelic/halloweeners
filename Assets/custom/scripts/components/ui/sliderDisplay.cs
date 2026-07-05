using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class sliderDisplay : MonoBehaviour {
    public Slider slider;
    private TMP_Text display;

    void OnEnable() {display = transform.GetComponent<TMP_Text>();}
    void Update() {display.text = (
        Mathf.Round(slider.value * 100) / 100
    ).ToString();}
}