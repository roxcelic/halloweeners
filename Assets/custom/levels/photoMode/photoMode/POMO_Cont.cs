using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using POMO;

public class POMO_Cont : MonoBehaviour {
    public static POMO_Cont self;

    /// <summery> basic variables </summery>
    [Header("base variables")]
    public Transform root;

    [Header("elements")]
    public GameObject button;
    public GameObject colorPicker;
    public GameObject hr;
    public GameObject text;
    public GameObject slider;
    public GameObject input;
    public GameObject vector;

    [Header("components")]
    public noclipcontroller cam;
    public GameObject retroCam;
    public GameObject spriteDisplay;
    public Light light;

    [Header("data")]
    public Sprite imageToDisplay;

    [Header("special buttons")]
    public POMO_ui_obj imageDisplayButton;

    /// <summery> on start </summery>
    void Start() {
        GS.live.state.pause(true); // pause the game
        self = this; //set the self

        new POMO_ui_obj(ui.types.button, new sys.Text("main menu"), (string interact, POMO_ui_obj self) => {
            SceneManager.LoadScene(0);
        }).interactor.buttonText.text = "";
        new POMO_ui_obj(ui.types.text, new sys.Text("photo mode ui"));
        new POMO_ui_obj(ui.types.colorPicker);
        new POMO_ui_obj(ui.types.hr);
        new POMO_ui_obj(ui.types.text, new sys.Text("config"));
        new POMO_ui_obj(ui.types.slider, new sys.Text("camera allowed distance"), (string interact, POMO_ui_obj self) => {
            cam.distance = 25f * self.interactor.slider.value;
        }).interactor.slider.value = 1;
        cam.distance = 25f;
        new POMO_ui_obj(ui.types.slider, new sys.Text("light"), (string interact, POMO_ui_obj self) => {
            float val = self.interactor.slider.value * 1000f;
            light.range = val;
            light.intensity = val * 5;
        }, true).interactor.slider.value = light.range / cam.distance;
        new POMO_ui_obj(ui.types.button, new sys.Text("free cam"), (string interact, POMO_ui_obj self) => {
            switch (self.interactor.buttonText.text) {
                case "on":
                    cam.freeCam = false;
                    self.interactor.buttonText.text = "off";
                    break;
                case "off": default:
                    cam.freeCam = true;
                    self.interactor.buttonText.text = "on";
                    break;
            }
        }).interactor.buttonText.text = cam.freeCam ? "on" : "off";
        new POMO_ui_obj(ui.types.button, new sys.Text("switch filter mode"), (string interact, POMO_ui_obj self) => {
            if (retroCam == null) return;

            switch (self.interactor.buttonText.text) {
                case "regular":
                    retroCam.SetActive(true);
                    self.interactor.buttonText.text = "retro";
                    break;
                case "retro": default:
                    retroCam.SetActive(false);
                    self.interactor.buttonText.text = "regular";
                    break;
            }
        }, true);

        new POMO_ui_obj(ui.types.hr);
        new POMO_ui_obj(ui.types.text, new sys.Text("focal image settings"));

        new POMO_ui_obj(ui.types.button, new sys.Text("open image directory"), (string interact, POMO_ui_obj self) => {
            Application.OpenURL($"{Application.persistentDataPath}/Images/");
        }).interactor.buttonText.text = "open";
        
        imageDisplayButton = new POMO_ui_obj(ui.types.button, new sys.Text("open image subMenu"), (string interact, POMO_ui_obj self) => {
            if (spriteDisplay == null) return;

            spriteDisplay.SetActive(true);
            self.interactor.buttonText.text = "";
        });
        imageDisplayButton.interactor.buttonText.text = "///";

        new POMO_ui_obj(ui.types.hr);
        new POMO_ui_obj(ui.types.text, new sys.Text("if you fuck up"));
        new POMO_ui_obj(ui.types.button, new sys.Text("reset camera position"), (string interact, POMO_ui_obj self) => {
            if (cam != null) {
                cam.transform.position = new Vector3(0, 2, -10);
                cam.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }).interactor.buttonText.text = "reset";
        new POMO_ui_obj(ui.types.hr);
        new POMO_ui_obj(ui.types.text, new sys.Text("dev"));
        new POMO_ui_obj(ui.types.input, new sys.Text("input test"), (string interact, POMO_ui_obj self) => {
            Debug.Log($"new value: {self.interactor.input.text}");
        });
        new POMO_ui_obj(ui.types.vector, new sys.Text("vector test"), (string interact, POMO_ui_obj self) => {
            Debug.Log($"new value: x: {self.interactor.VectorX.text} y: {self.interactor.VectorY.text} z: {self.interactor.VectorZ.text}");
        });
    }

}