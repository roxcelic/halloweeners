using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using ext;

public class pauseMenuController : MonoBehaviour {
    [Header("items")]
    public List<PM_Base> baseCommands;

    private List<PM_Base> currentItems;
    private List<List<PM_Base>> previousItems = new List<List<PM_Base>>();

    [Header("comp")]
    public TMP_InputField actualInput;
    public TMP_Text MainDisplay;
    public TMP_Text LogDisplay;
    public ScrollRect LogDisplaySR;

    // [Header("text")]
    // public sys.Text newOptionsMessage = new sys.Text();
    // public sys.Text introMessage = new sys.Text();

    // data
    [Header("data")]
    public bool interactable = true;
    public string responded = "";
    private int selectedIndex = 0;

    #region main
    void Start() {
        currentItems = baseCommands;
        displayText();
    }
    
    void Update() {
        if (!GS.live.state.loaded) return; // if the level isnt loaded dont let the player pause
        if (GS.live.state.helped) return; // if the game is in help mode dont allow pause
        if (GS.live.state.menued) return; // if the game is in menu mode dont allow pause

        if (eevee.input.Grab("Pause", "pm")) changePauseState(!GS.live.state.paused);
        if (!GS.live.state.paused) return;

        if (!interactable) return;

        if (eevee.input.Collect("down", "pm")) {selectedIndex++; if (selectedIndex > currentItems.Count - 1) selectedIndex = 0;}
        if (eevee.input.Collect("up", "pm")) {selectedIndex--; if (selectedIndex < 0) selectedIndex = currentItems.Count - 1;}
    
        if (eevee.input.Collect("interact", "pm")) {currentItems[selectedIndex].action(this, "");}
        if (eevee.input.Collect("back", "pm")) {loadPrevMenu();}

        displayText();
    }

    void FixedUpdate() {}
    #endregion

    #region utils
    /// <summery> logs info </summery>
    public void log(string content, string program = "user" ,string color = "red") {
        LogDisplay.text += $"\n<color={color}> {program}> {content} </color>";
        StartCoroutine(wait(() => {LogDisplaySR.ScrollToBottom();}, 0.001f));
    }

    /// <summery> change the state to paused </summery>
    public void changePauseState(bool newVal) {
        GS.live.state.pause(newVal);

        transform.GetChild(0).gameObject.SetActive(newVal);
    }

    /// <summery> displays the text </summery>
    public string displayText() {
        string result = "";
        for (int i = 0; i < currentItems.Count; i++) result += $"{(selectedIndex == i ? ">" : "")} {currentItems[i].name.localise()} \n";
        MainDisplay.text = result;
        return result;
    }

    /// <summery> select a new menu </summery>
    public void loadMenu(List<PM_Base> newItems, bool hide = false) {
        if (newItems.Count == 0) return;
        if(!hide) previousItems.Add(currentItems);
        currentItems = newItems;
        selectedIndex = 0;
    }

    /// <summery> load the previous menu </summery>
    public void loadPrevMenu() {
        if (previousItems.Count == 0) {
            changePauseState(false);
            return;
        }

        currentItems = previousItems[previousItems.Count - 1];
        previousItems.RemoveAt(previousItems.Count - 1);

        selectedIndex = 0;
    }

    /// <summery> gets a user input </summery>
    public async Task<string> getText(string placeHolder) {
        actualInput.transform.gameObject.SetActive(true);
        actualInput.ActivateInputField();
        actualInput.Select();
        actualInput.placeholder.transform.GetComponent<TMP_Text>().text = placeHolder;

        responded = "";
        interactable = false;

        while (responded == "") {await Task.Delay(50);}

        actualInput.transform.gameObject.SetActive(false);
        interactable = true;

        return responded;
    }

    /// <summery> allows an input to be given </summery>
    public void setTextRep() {responded = actualInput.text;}

    #endregion

    #region coRoutines
    /// <summery> allows for a js like setTimeout </summery>
    public IEnumerator wait(System.Action action, float Delay){
        yield return new WaitForSecondsRealtime(Delay);
        action();
    }
    #endregion
}