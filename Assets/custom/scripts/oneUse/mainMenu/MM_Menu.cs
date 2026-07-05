using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

public class MM_Menu : pauseMenuController {
    public static MM_Menu instance;
    public Transform subMenu;

    [Header("Main menu")]
    public Transform camera;

    protected override void Start() {
        base.Start();
        GS.live.state.paused = true;
        instance = this;
    }

    protected override void Update() {
        if (!interactable) return;

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

    /// <summery> load the previous menu </summery>
    public override void loadPrevMenu() {
        if (previousItems.Count == 0) return;

        currentItems = previousItems[previousItems.Count - 1];
        previousItems.RemoveAt(previousItems.Count - 1);

        selectedIndex = 0;
        displayText();
    }
}