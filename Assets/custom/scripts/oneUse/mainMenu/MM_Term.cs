using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class MM_Term : MonoBehaviour {
    public void open() {
        playerController.mainPlayer.pauseMenu.open();
    }
}