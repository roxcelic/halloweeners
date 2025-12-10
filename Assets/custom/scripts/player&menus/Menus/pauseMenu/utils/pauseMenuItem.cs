using UnityEngine;

using TMPro;

public class pauseMenuItem : MonoBehaviour {
    public bool selected = false;
    public MM_base menuItem;
    public pauseMenuController PMC;
    public TMP_Text text;

    public void Run() {
        menuItem.action(null, PMC);
    }
}