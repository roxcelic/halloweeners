using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class pauseMenuController : MonoBehaviour {
    [Header("items")]
    public List<PM_Base> currentItems;
    public List<PM_Base> baseCommands;

    public List<List<PM_Base>> previousItems = new List<List<PM_Base>>();

    [Header("components")]
    public TMP_InputField input;
    public TMP_Text text;
    public Animator anim;

    [Header("text")]
    public sys.Text newOptionsMessage = new sys.Text();
    public sys.Text introMessage = new sys.Text();

    void Start() {
        anim = transform.GetChild(0).GetComponent<Animator>();

        if (!save.getData.viewSave().firstTimeInPauseMenu) {
            log(introMessage.localise(), "system", "blue");
        }
    }

    void Update() {
        if (!GS.live.state.loaded) return; // if the level isnt loaded dont let the player pause
        if (GS.live.state.helped) return; // if the game is in help mode dont allow pause
        if (GS.live.state.menued) return; // if the game is in menu mode dont allow pause

        if (eevee.input.Grab("Pause", "pm")) changePauseState(!GS.live.state.paused);
        if (!GS.live.state.paused) return;

        if (eevee.conf.autoDetect() == eevee.inputCL.controller) {
            log($"controller used, up: {eevee.input.Collect("up", "pm")}", "log", "blue");
        }
    }

    #region utils
    // a util to assist with changing the pause state to turn the menu on and off 
    public void changePauseState(bool newVal) {
        if (!newVal) {
            input.ActivateInputField();
            input.Select();
        }

        GS.live.state.pause(newVal);
        if (newVal) {
            transform.GetChild(0).gameObject.SetActive(newVal);
            anim.Play("open");
        } else anim.Play("close");
    }

    // a util to run a "command"
    public void run(string command) {
        log(command);
        input.text = ""; // clear the inputs text, why am i commenting ts

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

        input.ActivateInputField();
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
    }

    public void loadPrevMenu() {
        currentItems = previousItems[previousItems.Count - 1];
        previousItems.RemoveAt(previousItems.Count - 1);
        logNewOptions(currentItems);
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
}