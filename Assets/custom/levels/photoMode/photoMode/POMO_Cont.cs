using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using POMO;

using ext;

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

    public GameObject spriteItem;

    [Header("components")]
    public noclipcontroller cam;
    public GameObject retroCam;
    public GameObject emptyCam;
    public GameObject spriteDisplay;
    public Light light;

    [Header("data")]
    public Sprite imageToDisplay;

    [Header("var")]
    public GameObject currentMap;
    public GameObject currentFocus;

    /// <summery> on start </summery>
    void Start() {
        GS.live.state.pause(true); // pause the game
        self = this; //set the self

        #region Main
            new POMO_ui_obj(ui.types.button, new sys.Text("main menu"), (string interact, POMO_ui_obj self) => {
                SceneManager.LoadScene(0);
            });
            new POMO_ui_obj(ui.types.text, new sys.Text("photo mode ui"));
            new POMO_ui_obj(ui.types.colorPicker);
        #endregion 

        new POMO_ui_obj(ui.types.hr);

        #region config
            new POMO_ui_obj(ui.types.text, new sys.Text("config"));

            cam.distance = 25f;

            new POMO_ui_obj(ui.types.slider, new sys.Text("camera allowed distance"), (string interact, POMO_ui_obj self) => {
                self.onRespawn = () => {
                    self.interactor.slider.value = cam.distance / 25f;
                };

                cam.distance = 25f * self.interactor.slider.value;
            }, true).onRespawn();

            new POMO_ui_obj(ui.types.slider, new sys.Text("light"), (string interact, POMO_ui_obj self) => {
                self.onRespawn = () => {
                    self.interactor.slider.value = light.range / 1000f;
                };
                
                float val = self.interactor.slider.value * 1000f;
                light.range = val;
                light.intensity = val * 5;

            }, true).onRespawn();

            new POMO_ui_obj(ui.types.button, new sys.Text("free cam"), (string interact, POMO_ui_obj self) => {
                switch (self.interactor.buttonText.text) {
                    case "on":
                        cam.freeCam = false;
                        self.interactor.buttonText.text = "off";
                        self.onRespawn = () => {self.interactor.buttonText.text = "off";};
                        break;
                    case "off": default:
                        cam.freeCam = true;
                        self.interactor.buttonText.text = "on";
                        self.onRespawn = () => {self.interactor.buttonText.text = "on";};
                        break;
                }
            }, true);

            new POMO_ui_obj(ui.types.button, new sys.Text("switch filter mode"), (string interact, POMO_ui_obj self) => {
                if (retroCam == null) return;

                switch (self.interactor.buttonText.text) {
                    case "regular":
                        retroCam.SetActive(true);
                        emptyCam.SetActive(false);
                        self.interactor.buttonText.text = "retro";
                        self.onRespawn = () => {self.interactor.buttonText.text = "retro";};
                        break;
                    case "retro":
                        retroCam.SetActive(false);
                        emptyCam.SetActive(true);
                        self.interactor.buttonText.text = "no filter";
                        self.onRespawn = () => {self.interactor.buttonText.text = "no filter";};
                        break;
                    case "no filter": default:
                        retroCam.SetActive(false);
                        emptyCam.SetActive(false);
                        self.interactor.buttonText.text = "regular";
                        self.onRespawn = () => {self.interactor.buttonText.text = "regular";};
                        break;
                }

            }, true);
        #endregion

        new POMO_ui_obj(ui.types.hr);

        #region dev
            new POMO_ui_obj(ui.types.text, new sys.Text("if you fuck up"));
            new POMO_ui_obj(ui.types.button, new sys.Text("reset camera position"), (string interact, POMO_ui_obj self) => {
                if (cam != null) {
                    cam.transform.position = new Vector3(0, 2, -10);
                    cam.transform.eulerAngles = new Vector3(0, 0, 0);
                }

                self.onRespawn = () => {
                    self.interactor.buttonText.text = "reset";
                };
            }, true).onRespawn();
        #endregion

            new POMO_ui_obj(ui.types.hr);

        #region sprites
            new POMO_ui_obj(ui.types.text, new sys.Text("sprites"));   
            new POMO_ui_obj(ui.types.button, new sys.Text("open sprite folder"), (string interact, POMO_ui_obj self) => {
                Application.OpenURL($"{Application.persistentDataPath}/Images/");
            });
            new POMO_ui_obj(ui.types.button, new sys.Text("add new sprite"), (string interact, POMO_ui_obj self) => {
                List<POMO_ui_obj> childrenObjects = new List<POMO_ui_obj>();
                List<POMO_ui_obj> frames = new List<POMO_ui_obj>();

                GameObject subject = GameObject.Instantiate(spriteItem, new Vector3(), Quaternion.identity);
                subject.transform.parent = currentMap.transform;
                
                POMO_Animator animator = subject.transform.GetComponent<POMO_Animator>();

                Light SUB_light = subject.transform.GetComponent<Light>();
                SUB_light.range = 0f;
                SUB_light.intensity = 0f;

                setColor SUB_color = subject.transform.GetComponent<setColor>();

                // make objects
                childrenObjects.Add(self.insertChildBelow(ui.types.hr));

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.text, new sys.Text("transform")
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.vector, new sys.Text("position"), (string interact, POMO_ui_obj self) => {
                            self.onRespawn = () => {
                                Vector3 vec = subject.transform.localPosition;
                                
                                self.interactor.VectorX.text = $"{vec.x}";
                                self.interactor.VectorY.text = $"{vec.y}";
                                self.interactor.VectorZ.text = $"{vec.z}";
                            };

                            subject.transform.localPosition = self.getVector();
                        }, true
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.vector, new sys.Text("scale"), (string interact, POMO_ui_obj self) => {
                            self.onRespawn = () => {
                                Vector3 vec = subject.transform.localScale;

                                self.interactor.VectorX.text = $"{vec.x}";
                                self.interactor.VectorY.text = $"{vec.y}";
                                self.interactor.VectorZ.text = $"{vec.z}";
                            };

                            subject.transform.localScale = self.getVector();
                        }, true
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.button, new sys.Text("effected By Filter"), (string interact, POMO_ui_obj self) => {
                            switch(self.interactor.buttonText.text) {
                                case "yes":
                                    self.onRespawn = () => {self.interactor.buttonText.text = "no";};
                                    self.interactor.buttonText.text = "no";
                                    subject.layer = sys.var.layers.altGround;
                                    break;
                                case "no": default:
                                    self.onRespawn = () => {self.interactor.buttonText.text = "yes";};
                                    self.interactor.buttonText.text = "yes";
                                    subject.layer = sys.var.layers.ground;
                                    break;
                            }
                        }, true
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.vector, new sys.Text("rotation"), (string interact, POMO_ui_obj self) => {
                            self.onRespawn = () => {
                                Vector3 vec = subject.transform.localEulerAngles;

                                self.interactor.VectorX.text = $"{vec.x}";
                                self.interactor.VectorY.text = $"{vec.y}";
                                self.interactor.VectorZ.text = $"{vec.z}";
                            };

                            subject.transform.localEulerAngles = self.getVector();
                        }, true
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.text, new sys.Text("animation")
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.input, new sys.Text("frame duration"), (string interact, POMO_ui_obj self) => {
                            if (animator == null) return;

                            self.onRespawn = () => {
                                self.interactor.input.text = $"{animator.frameDuration}";
                            };

                            animator.frameDuration = self.interactor.input.getFloatValue(0.1f);     
                        }, true
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.button, new sys.Text("add frame"), (string interact, POMO_ui_obj self) => {
                            if (animator == null) return;

                            int frameNum = animator.sprites.Count;
                            animator.sprites.Add(null);

                            POMO_ui_obj chosen = frames.Count == 0 ? self : frames[frames.Count - 1];
                            POMO_ui_obj frame = chosen.insertChildBelow(
                                ui.types.button, new sys.Text($"frame {frameNum}"), (string interact, POMO_ui_obj self) => {
                                    activateImageDisplay((Sprite image) => {
                                        self.interactor.buttonDisplay.sprite = image;
                                        self.interactor.buttonDisplay.transform.gameObject.SetActive(true);
                                        self.interactor.buttonText.text = "";

                                        animator.sprites[frameNum] = image;

                                        self.onRespawn = () => {
                                            self.interactor.buttonDisplay.sprite = image;
                                            self.interactor.buttonDisplay.transform.gameObject.SetActive(true);
                                            self.interactor.buttonText.text = "";
                                        };
                                    });
                                }
                            );

                            frames.Add(frame);
                            childrenObjects.Add(frame);

                            utils.Regenerate();
                        }
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.button, new sys.Text("remove frame"), (string interact, POMO_ui_obj self) => {
                            if (animator == null || animator.sprites.Count == 0) return;

                            animator.sprites.RemoveAt(animator.sprites.Count - 1);
                            frames[frames.Count - 1].kill();
                            childrenObjects.Remove(frames[frames.Count -1 ]);
                            frames.RemoveAt(frames.Count - 1);
                        }
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.text, new sys.Text("light")
                    )
                );

                
                if (SUB_light != null) {
                    POMO_ui_obj lightObj = childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.slider, new sys.Text("light strength"), (string interact, POMO_ui_obj self) => {
                            self.onRespawn = () => {
                                self.interactor.slider.value = SUB_light.range / 1000f;
                            };

                            SUB_light.range = self.interactor.slider.value * 1000f;
                            SUB_light.intensity = self.interactor.slider.value * 5000f;
                        }, true
                    );
                    lightObj.onRespawn();

                    childrenObjects.Add(lightObj);
                }


                if (SUB_color != null) {
                    POMO_ui_obj colorObj = childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.button, new sys.Text("change light color"), (string interact, POMO_ui_obj self) => {
                            self.onRespawn = () => {self.interactor.buttonText.text = "";};

                            switch (self.interactor.buttonText.text) {
                                case "world color":
                                    self.onRespawn = () => {self.interactor.buttonText.text = "alt color";};
                                    self.interactor.buttonText.text = "alt color";

                                    SUB_color.active = true;
                                    SUB_color.invertColor = true;
                                    break;
                                case "alt color":
                                    self.onRespawn = () => {self.interactor.buttonText.text = "white";};
                                    self.interactor.buttonText.text = "white";
                                    SUB_color.active = false;
                                    SUB_color.invertColor = false;
                                    SUB_light.color = Color.white;
                                    break;
                                case "white": default:
                                    self.onRespawn = () => {self.interactor.buttonText.text = "world color";};
                                    self.interactor.buttonText.text = "world color";
                                    SUB_color.active = true;
                                    SUB_color.invertColor = false;
                                    break;
                            }

                        }, true
                    );

                    colorObj.onRespawn();

                    childrenObjects.Add(colorObj);
                }

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.text, new sys.Text("settings")
                    )
                );

                childrenObjects.Add(
                    childrenObjects[childrenObjects.Count - 1].insertChildBelow(
                        ui.types.button, new sys.Text("focus sprite"), (string interact, POMO_ui_obj self) => {
                            if (cam.lookat == subject.transform) cam.lookat = currentFocus.transform;
                            else cam.lookat = subject.transform;
                        }
                    )
                );

                // destroy button
                childrenObjects[childrenObjects.Count - 1].insertChildBelow(ui.types.button, new sys.Text("destroy"), (string interact, POMO_ui_obj self) => {
                    foreach(POMO_ui_obj child in childrenObjects) child.kill();
                    self.kill();

                    Destroy(subject);

                    utils.Regenerate();
                });

                utils.Regenerate();

            });

            new POMO_ui_obj(ui.types.hr);
        #endregion
    }

    #region utils
    
    public void activateImageDisplay(System.Action<Sprite> act) {
        if (act == null || POMO_ImageDisplay_Cont.self == null) return;

        POMO_ImageDisplay_Cont.self.enable(true);
        POMO_ImageDisplay_Cont.self.loadedAction = act;
    }

    #endregion

}