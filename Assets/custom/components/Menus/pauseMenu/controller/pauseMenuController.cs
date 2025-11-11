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
    public List<PM_Base> currentItems;
    public List<PM_Base> baseCommands;

    public List<List<PM_Base>> previousItems = new List<List<PM_Base>>();

    [Header("components")]
    public TMP_InputField actualInput;
    public TMP_Text input;
    public TMP_Text text;
    public Animator anim;
    public ScrollRect SR;

    [Header("text")]
    public sys.Text newOptionsMessage = new sys.Text();
    public sys.Text introMessage = new sys.Text();

    [Header("data")]
    public int index = 0;
    public string responded = "";
    public bool interactable = true;

    void Start() {
        anim = transform.GetChild(0).GetComponent<Animator>();

        if (!save.getData.viewSave().firstTimeInPauseMenu) {
            log(introMessage.localise(), "system", "blue");
        }
    
        updateDisplay();
    }

    async void Update() {
        if (!GS.live.state.loaded) return; // if the level isnt loaded dont let the player pause
        if (GS.live.state.helped) return; // if the game is in help mode dont allow pause
        if (GS.live.state.menued) return; // if the game is in menu mode dont allow pause

        if (eevee.input.Grab("Pause", "pm")) changePauseState(!GS.live.state.paused);
        if (!GS.live.state.paused) return;

        if (!interactable) return;

        if (eevee.input.Collect("down", "pm")) {index++; if (index > orderCommands().Count - 1) index = 0;updateDisplay();}
        if (eevee.input.Collect("up", "pm")) {index--; if (index < 0) index = orderCommands().Count - 1;updateDisplay();}
        if (eevee.input.Collect("interact", "pm")) {orderCommands()[index].action(this, "");}
        if (eevee.input.Collect("back", "pm")) {findCommand("back", orderCommands()).action(this, "");}

        if (eevee.input.Collect("special", "pm")) {
            PM_Base command = findCommand(await getText(""), orderCommands());
            if (command == null) return;
            command.action(this, "");
        }
    }

    #region utils
    // a util to load the current text
    public void updateDisplay() {input.text = $"> {orderCommands()[index].name.localise()}";}

    // a util to get a text input
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

    // a util to assist with changing the pause state to turn the menu on and off 
    public void changePauseState(bool newVal) {
        GS.live.state.pause(newVal);
        if (newVal) {
            transform.GetChild(0).gameObject.SetActive(newVal);
            anim.Play("open");
        } else anim.Play("close");
    }

    // a util to set the text response
    public void setTextRep() {responded = actualInput.text;}

    // a util to run a "command"
    public void run(string command) {
        log(command);

        string[] commandData = command.Split(" ");
        string inputString = ""; 
        if (commandData.Length > 1) inputString = command.Substring(commandData[0].Length + 1);

        switch(commandData[0]) {
            case "exit":
                changePauseState(false);

                break;
            default:
                // command run statement
                PM_Base chosenCommand = findCommand(commandData[0], baseCommands);
                if (chosenCommand != null) {
                    chosenCommand.action(this);
                } else {
                    chosenCommand = findCommand(commandData[0], currentItems);
                    if (chosenCommand != null) {
                        chosenCommand.action(this, inputString);
                    }
                }

                break;
        }
    }

    // a util command to find and return the menu item if it exists
    public PM_Base findCommand(string name, List<PM_Base> search) {
        foreach (PM_Base Mitem in search) {
            if (Mitem.name.localise() == name && (!Mitem.dev || save.getData.isDev())) return Mitem;
        }

        return null;
    }

    // a util to log
    public void log(string content, string program = "user" ,string color = "red") {
        text.text += $"\n<color={color}> {program}> {content} </color>";
        StartCoroutine(wait(() => {SR.ScrollToBottom();}, 0.001f));
    }

    // a util to get an ordered list of commands
    public List<PM_Base> orderCommands() {
        List<PM_Base> orderedList = new List<PM_Base>();
        List<PM_Base> priorityItems = new List<PM_Base>();

        foreach(PM_Base Mitem in baseCommands) {
            if (Mitem.essential) priorityItems.Add(Mitem);
            else orderedList.Add(Mitem);
        }

        foreach (PM_Base Mitem in currentItems) {
            if (Mitem.essential) priorityItems.Add(Mitem);
            else orderedList.Add(Mitem);
        }

        foreach (PM_Base Mitem in orderedList) {
            priorityItems.Add(Mitem);
        }

        return priorityItems;
    }

    // a util to load a new menu
    public void loadMenu(List<PM_Base> newItems, bool hide = false) {
        if (newItems.Count == 0) return;

        if(!hide) previousItems.Add(currentItems);
        
        // log new options
        logNewOptions(newItems);

        currentItems = newItems;

        index = 0;
        updateDisplay();
    }

    public void loadPrevMenu() {
        if (previousItems.Count == 0) {
            changePauseState(false);

            return;
        }

        currentItems = previousItems[previousItems.Count - 1];
        previousItems.RemoveAt(previousItems.Count - 1);
        logNewOptions(currentItems);

        index = 0;
        updateDisplay();
    }

    // a util to log options
    void logNewOptions(List<PM_Base> newItems) {
        log("-----", "", "white");
        log(newOptionsMessage.localise(), "system", "blue");
        foreach (PM_Base Mitem in newItems) {
            Mitem.onLoad(this); // sneaky
            log($"\t{Mitem.name.localise()}", "system", "blue");
        }
        log("-----", "", "white");
    }

    #endregion

    #region coroutines
    public IEnumerator wait(System.Action action, float Delay){
        yield return new WaitForSecondsRealtime(Delay);
        action();
    }
    #endregion
}