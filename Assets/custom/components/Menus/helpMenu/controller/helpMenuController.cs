using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace helpMenuClasses {
    [System.Serializable]
    public class info {

    }
}

public class helpMenuController : MonoBehaviour {
    [Header("components")]
    public Animator anim;

    public TMP_Text Header;
    public TMP_Text body;
    public Image sprite;

    void Start() {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    void Update() {
        if (!GS.live.state.loaded) return; // if the level isnt loaded dont let the player pause

        if (Input.GetButtonDown("help")) changePauseState(!GS.live.state.paused);
        if (!GS.live.state.paused) return;
    }

    #region utils
        // a util to assist with changing the pause state to turn the menu on and off 
        public void changePauseState(bool newVal) {
            Cursor.lockState = (newVal ? CursorLockMode.None : CursorLockMode.Locked);
            Cursor.visible = newVal;

            GS.live.state.pause(newVal);
            if (newVal) {
                transform.GetChild(0).gameObject.SetActive(newVal);
                anim.Play("open");
            } else anim.Play("close");
        }
    #endregion
}