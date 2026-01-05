using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class MM_Term : pauseMenuController {
    [Header("main Menu")]
    public Transform newCamPos;
    private bool opened = false;
    private Vector3 camStart;

    protected override void Start() {
        instance = this;
        currentItems = baseCommands;
        displayText();
    }

    protected override void Update() {
        if (!interactable) {
            MainDisplay.text = "Start";
            return;
        }

        if (eevee.input.Collect("down", "pm")) {selectedIndex++; if (selectedIndex > getPirvlagedOptions().Count - 1) selectedIndex = 0;displayText();}
        if (eevee.input.Collect("up", "pm")) {selectedIndex--; if (selectedIndex < 0) selectedIndex = getPirvlagedOptions().Count - 1;displayText();}
    
        if (eevee.input.Collect("interact", "pm")) {
            getPirvlagedOptions()[selectedIndex].action(this, "");
            displayText();
        }
        if (eevee.input.Collect("back", "pm")) {loadPrevMenu();}

        alignTextBox();
    }

    public override void alignTextBox() {
        MainDisplayRect.localPosition = Vector3.Lerp(
            MainDisplayRect.localPosition, 
            new Vector3(
                MainDisplayRect.localPosition.x,
                Mathf.Clamp(selectedIndex - ignorance, 0, Mathf.Infinity) * textHeight, 
                MainDisplayRect.localPosition.z
            ), 
            Time.fixedDeltaTime * 5
        );
    }

    /// <summery> load the previous menu </summery>
    public override void loadPrevMenu() {
        if (previousItems.Count == 0) {
            close();
            return;
        }

        currentItems = previousItems[previousItems.Count - 1];
        previousItems.RemoveAt(previousItems.Count - 1);

        selectedIndex = 0;
        displayText();
    }

    public void open() {
        if (opened) return;
        opened = true;
        camStart = playerController.mainPlayer.camera.position;
        StartCoroutine(moveCam());
    }

    public void close() {
        StartCoroutine(moveCamBack());
    }

    public IEnumerator moveCam() {
        playerController.mainPlayer.CanMove = false;
        Transform cam = playerController.mainPlayer.camera;
        Time.timeScale = 0f;

        while (Vector3.Distance(cam.position, newCamPos.position) > 0.05f) {
            cam.position = Vector3.Lerp(cam.position, newCamPos.position, Time.fixedDeltaTime * 5f);
            yield return 0;
        }
        cam.position = newCamPos.position;

        interactable = true;
        displayText();
    }

    public IEnumerator moveCamBack() {
        Transform cam = playerController.mainPlayer.camera;
        interactable = false;

        while (Vector3.Distance(cam.localPosition, new Vector3()) > 0.05f) {
            cam.localPosition = Vector3.Lerp(cam.localPosition, new Vector3(), Time.fixedDeltaTime * 5f);
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.identity, Time.fixedDeltaTime * 15f);
            yield return 0;
        }
        cam.localRotation = Quaternion.identity;
        cam.localPosition = new Vector3();

        playerController.mainPlayer.CanMove = true;
        Time.timeScale = 1f;
        opened = false;
    }
}