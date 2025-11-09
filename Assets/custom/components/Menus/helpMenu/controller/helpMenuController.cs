using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace helpMenuClasses {
    [System.Serializable]
    public class info {
        public sys.Text heading;
        public sys.Text body;
        public Sprite display;

        public info() {
            // this.heading.English = "empty";
            // this.body.English = "";
            this.display = null;
        }
    }
}

public class helpMenuController : MonoBehaviour {
    [Header("components")]
    public Animator anim;

    public TMP_Text Header;
    public TMP_Text body;
    public Image sprite;

    [Header("data")]
    public List<helpMenuClasses.info> data;
    public int index = 0;

    void Start() {
        anim = transform.GetChild(0).GetComponent<Animator>();

        // start
        index = 0;
        displayText();
    }

    void Update() {
        if (!GS.live.state.loaded) return; // if the level isnt loaded dont let the player pause
        if (GS.live.state.paused) return; // dont let the player open help if the games paused
        if (GS.live.state.menued) return; // dont let the player open help if the games paused

        if (Input.GetButtonDown("help")) changePauseState(!GS.live.state.helped);
        if (!GS.live.state.helped) return;
    }

    #region utils
        // a util to assist with changing the pause state to turn the menu on and off 
        public void changePauseState(bool newVal) {
            GS.live.state.help(newVal);
            if (newVal) {
                transform.GetChild(0).gameObject.SetActive(newVal);
                anim.Play("open");
            } else anim.Play("close");
        }

        // a util to display the current text
        void displayText() {
            helpMenuClasses.info newHelp = null;
            if (index >= data.Count) newHelp = new helpMenuClasses.info();
            else newHelp = data[index];

            Header.text = sys.text.displayKeyButton(data[index].heading.localise());
            body.text = sys.text.displayKeyButton(data[index].body.localise());

            if (newHelp.display != null) sprite.sprite = newHelp.display;
        }

        // a util to move left
        public void moveLeft() {
            Debug.Log("left");
            index--;
            if (index < 0) index = data.Count -1;

            displayText();
        }

        // a util to move right
        public void moveRight() {
            Debug.Log("right");
            index++;
            if (index >= data.Count) index = 0;

            displayText();
        }
    #endregion
}