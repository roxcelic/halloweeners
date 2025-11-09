using UnityEngine;

using TMPro;

public class IMP_controller : MonoBehaviour {
    [Header("components")]
    public GameObject textLog;

    [Header("data")]
    public GameObject currentLog;

    void Update() {
        if (GS.live.state.menued || GS.live.state.helped) return;
        
        GameObject target = transform.GetChild(0).gameObject;
        if (eevee.input.Collect("Pause", "IPM")) {
            bool apply = !target.activeSelf;
            target.SetActive(apply);
            GS.live.state.pause(apply);
        }

        if (eevee.input.Collect("interact", "IPM") && currentLog != null) Destroy(currentLog); // close the log
    }

    #region utils
    // grab the current text display
    public TMP_Text getLog() {
        if (currentLog == null) currentLog = Instantiate(textLog);
        return currentLog.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void log(string input, string color = "#C0000D") {getLog().text = $"<color={color}>{input}</color>";} // a log command
    #endregion
}