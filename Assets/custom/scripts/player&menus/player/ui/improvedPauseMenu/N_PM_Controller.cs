using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using ext;

public class pauseMenuController : MonoBehaviour {
    public static pauseMenuController instance;

    [Header("items")]
    public List<PM_Base> baseCommands;
    public List<PM_Base> permaCommands;

    public List<PM_Base> currentItems;
    protected List<List<PM_Base>> previousItems = new List<List<PM_Base>>();

    [Header("comp")]
    public TMP_InputField actualInput;
    public Slider sliderVal;

    public TMP_Text MainDisplay;
    public RectTransform MainDisplayRect;
    public TMP_Text LogDisplay;
    public ScrollRect LogDisplaySR;

    public Animator anim;

    [Header("config")]
    public float textHeight = 15f; // this is for the offset when selecting options
    public int ignorance = 5;

    // data
    [Header("data")]
    public bool interactable {
        get { return IHLD; }
        set {
            if (value) StartCoroutine(wait(() => {
                displayText();
            }, 0f)); // wait 1 frame twin
            
            IHLD = value;
        }
    }
    public bool IHLD = true;
    
    public string responded = "";
    public float? respondedSlider = -1;

    public int selectedIndex = 0;
    public int hoveredIndex = 0;

    [Header("sub menus")]
    public GameObject colorPicker;
    public GameObject vault;

    #region main
    protected virtual void Start() {
        instance = this;
        anim = transform.GetComponent<Animator>();
        currentItems = baseCommands;
        displayText();
        MainDisplayRect = MainDisplay.transform.GetComponent<RectTransform>();
    }
    
    protected virtual void Update() {
        if (!GS.live.state.loaded) return; // if the level isnt loaded dont let the player pause
        if (GS.live.state.helped) return; // if the game is in help mode dont allow pause
        if (GS.live.state.menued) return; // if the game is in menu mode dont allow pause

        if (eevee.input.Grab("Pause", "pm")) changePauseState(!GS.live.state.paused);
        if (!GS.live.state.paused) return;

        if (!interactable) return;
        if (colorPicker.activeSelf || vault.activeSelf) return;

        if (eevee.input.Collect("down", "pm") || Input.GetAxis("Mouse ScrollWheel") < 0f) {
            if (hoveredIndex == -1) selectedIndex++;
            else {
                selectedIndex = hoveredIndex + 1;
                hoveredIndex = -1;
            }

            if (selectedIndex > getPirvlagedOptions().Count - 1) 
                selectedIndex = 0;
            
            displayText();
        }
        if (eevee.input.Collect("up", "pm") || Input.GetAxis("Mouse ScrollWheel") > 0f) {
            if (hoveredIndex == -1) selectedIndex--;
            else {
                selectedIndex = hoveredIndex - 1;
                hoveredIndex = -1;
            }

            if (selectedIndex < 0) 
                selectedIndex = getPirvlagedOptions().Count - 1;
        
            displayText();
        }
    
        if (((eevee.input.Collect("interact", "pm"))&& getPirvlagedOptions()[selectedIndex].active())) {getPirvlagedOptions()[selectedIndex].action(this, "");displayText();}
        if (eevee.input.Collect("back", "pm")) {loadPrevMenu();}

        alignTextBox();
    }

    void FixedUpdate() {}
    #endregion

    #region utils
    /// <summery> runs a click </summery>
    public void runOption(int option) {
        if (getPirvlagedOptions().Count <= option) return;

        if (getPirvlagedOptions()[option].active()) {
            getPirvlagedOptions()[option].action(this, "");
            displayText();
        }
    }

    /// <summery> logs info </summery>
    public void log(string content, string program = "user", string color = "red", bool thing = true) {
        LogDisplay.text += $"\n<color={color}> {program}{(thing ? ">" : "")} {content} </color>";
        StartCoroutine(wait(() => {LogDisplaySR.ScrollToBottom();}, 0.001f));
    }

    /// <summery> resets and closes the menu </summery>
    public virtual void reset() {
        Start();
        previousItems = new List<List<PM_Base>>();
    }

    /// <summery> asks yes or no </summery>
    public void question(System.Action act) {
        PM_yes yes = ScriptableObject.CreateInstance("PM_yes") as PM_yes;
        yes.followUp = act;
        PM_Base no = ScriptableObject.CreateInstance("PM_no") as PM_no;

        loadMenu(new List<PM_Base>{yes, no});
    }
    
    /// <summery> a util to open/close the menu </summery>
    public void open() {
        changePauseState(true);
    }

    /// <summery> change the state to paused </summery>
    public void changePauseState(bool newVal) {
        GS.live.state.pause(newVal);

        transform.GetChild(0).gameObject.SetActive(newVal);
    }

    /// <summery> displays the text </summery>
    public string displayText() {
        string result = "";
        
        if (hoveredIndex == -1) {
            for (int i = 0; i < getPirvlagedOptions().Count; i++) {
                getPirvlagedOptions()[i].onLoad(this);
                result += $"<link=\"{i}\">{(selectedIndex == i ? ">" : (i < selectedIndex ? "|" : ""))}<color={(getPirvlagedOptions()[i].active() ? "white" : "grey" )}> {getPirvlagedOptions()[i].getName()} </color> </link>\n";
            }
        } else {
            for (int i = 0; i < getPirvlagedOptions().Count; i++) {
                getPirvlagedOptions()[i].onLoad(this);
                result += $"<link=\"{i}\">{(hoveredIndex == i ? ">" : (i < hoveredIndex ? "|" : ""))}<color={(getPirvlagedOptions()[i].active() ? "white" : "grey" )}> {getPirvlagedOptions()[i].getName()} </color> </link>\n";
            }
        }

        MainDisplay.text = result;
        return result;
    }

    /// <summery> get the list of options the user has privlage to </summery>
    public List<PM_Base> getPirvlagedOptions() {
        List<PM_Base> finalList = new List<PM_Base>();

        foreach (PM_Base item in currentItems) if (!item.dev || save.getData.isDev()) finalList.Add(item);
        if(previousItems.Count > 0) foreach (PM_Base item in permaCommands) finalList.Add(item);

        return finalList;
    }

    /// <summery> select a new menu </summery>
    public void loadMenu(List<PM_Base> newItems, bool hide = false) {
        if (newItems.Count == 0) return;
        if(!hide) previousItems.Add(currentItems);
        foreach (PM_Base item in newItems) item.runOnLoad();

        currentItems = newItems;
        selectedIndex = 0;
        hoveredIndex = 0;
        displayText();
    }

    /// <summery> load the previous menu </summery>
    public virtual void loadPrevMenu() {
        if (previousItems.Count == 0) {
            changePauseState(false);
            return;
        }

        currentItems = previousItems[previousItems.Count - 1];
        previousItems.RemoveAt(previousItems.Count - 1);

        selectedIndex = 0;
        hoveredIndex = 0;
        displayText();
    }

    /// <summery> gets a user input </summery>
    public async Task<string> getText(string placeHolder, string defaultAnswer = "") {
        Debug.Log($"attempting to get text, actualInput: {actualInput == null}");
        if (actualInput == null) return "";

        editInputAllowence(0, () => {
            actualInput.transform.gameObject.SetActive(true);
            actualInput.ActivateInputField();
            actualInput.Select();
            actualInput.placeholder.transform.GetComponent<TMP_Text>().text = placeHolder;
            responded = "";
        });

        bool regularPath = true;
        Debug.Log("waiting for result");
        while (responded == "" && GS.live.state.paused) await Task.Delay(50);
        Debug.Log("got result");

        editInputAllowence(1, () => {
            actualInput.transform.gameObject.SetActive(false);
        });

        return regularPath ? responded : defaultAnswer;
    }

    /// <summery> gets a user input slider </summery>
    public async Task<float> getValueSlider(float start, float max, float min = 0.5f) {
        if (sliderVal == null) return -1;

        /*
            Now you may ask why i made that a lambda function when i could have just ran it after,
            'why wouldnt you?' is the real question
        */
        editInputAllowence(0, () => {
            sliderVal.minValue = min;
            sliderVal.maxValue = max;
            sliderVal.value = start;
            sliderVal.transform.gameObject.SetActive(true);
            sliderVal.Select();
        });

        float slideTrack = start;

        bool regularPath = true;
        while ((sliderVal.value == start && (regularPath = !eevee.input.Check("back"))) && GS.live.state.paused) await Task.Delay(50);
        
        if (regularPath) { while (true) {
            if (slideTrack == sliderVal.value) break;
            
            slideTrack = sliderVal.value;
            await Task.Delay(250);
        } } else slideTrack = sliderVal.value;

        editInputAllowence(1, () => {
            sliderVal.transform.gameObject.SetActive(false);
        });

        return slideTrack;
    }

    /// <summery> a util to change the allowence of certain ui data </summery>
    public void editInputAllowence(int mode = 0, System.Action special = null, bool curserLock = false) {
        switch (mode) {
            case 1:
                if (curserLock) {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                playerController.mainPlayer.canMoveCamera = true;
                StartCoroutine(reAllowInput());

                break;
            case 0: default:
                if (curserLock) {
                    Cursor.lockState =  CursorLockMode.None;
                    Cursor.visible = true;
                }
                playerController.mainPlayer.canMoveCamera = false;
                interactable = false;
        
                break;
        }

        if(special != null) special();
    }

    /// <summery> check if a option is hovered </summery>
    public bool isSelected(PM_Base target) {
        return getPirvlagedOptions()[selectedIndex] == target;
    }

    /// <summery> allows an input to be given </summery>
    public void setTextRep() {responded = actualInput.text;}

    /// <summery> allows a slider input to be given </summery>
    public void setSliderRep() {respondedSlider = sliderVal.value;}

    /// <summery> aligns the text box correctly </summery>
    public virtual void alignTextBox() {
        MainDisplayRect.localPosition = Vector3.Lerp(MainDisplayRect.localPosition, new Vector3(MainDisplayRect.localPosition.x, Mathf.Clamp(selectedIndex - ignorance, 0, Mathf.Infinity) * textHeight, MainDisplayRect.localPosition.z), Time.fixedDeltaTime * 5);
    }

    #endregion

    #region coRoutines
    /// <summery> allows for a js like setTimeout </summery>
    public IEnumerator wait(System.Action action, float Delay){
        yield return new WaitForSecondsRealtime(Delay);
        action();
    }

    /// <summery> wait 0.35 (default) seconds to allow input again </summery>
    public IEnumerator reAllowInput(float duration = 0.35f) {
        yield return new WaitForSecondsRealtime(duration);
        interactable = true;
    } public void allowInput() {StartCoroutine(reAllowInput());} // a util to run this
    #endregion
}