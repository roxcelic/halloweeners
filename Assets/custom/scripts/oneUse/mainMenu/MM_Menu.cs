using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class MM_Menu : pauseMenuController {
    protected override void Update() {
        if (!interactable) return;

        if (eevee.input.Collect("down", "pm") || Input.GetAxis("Mouse ScrollWheel") < 0f) {selectedIndex++; if (selectedIndex > getPirvlagedOptions().Count - 1) selectedIndex = 0;displayText();}
        if (eevee.input.Collect("up", "pm") || Input.GetAxis("Mouse ScrollWheel") > 0f) {selectedIndex--; if (selectedIndex < 0) selectedIndex = getPirvlagedOptions().Count - 1;displayText();}
    
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