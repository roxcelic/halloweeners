using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class ControlInjector : MonoBehaviour {

    public bool overwrite = false;

    public Dictionary<string, eevee.config> config = new Dictionary<string, eevee.config>() {
        // directions
        {
            "right", new eevee.config {
                displayName = "right",
                KEYBOARD_code = new int[] {(int)KeyCode.D},
                CONTROLLER_name = new string[] {"Left Stick Right"}
            }
        },
        {
            "left", new eevee.config {
                displayName = "left",
                KEYBOARD_code = new int[] {(int)KeyCode.A},
                CONTROLLER_name = new string[] {"Left Stick Left"}
            }
        },
        {
            "up", new eevee.config {
                displayName = "up",
                KEYBOARD_code = new int[] {(int)KeyCode.W},
                CONTROLLER_name = new string[] {"Left Stick Up"}
            }
        },
        {
            "down", new eevee.config {
                displayName = "down",
                KEYBOARD_code = new int[] {(int)KeyCode.S},
                CONTROLLER_name = new string[] {"Left Stick Down"}
            }
        },

        {
            "Attack", new eevee.config {
                displayName = "Attack",
                KEYBOARD_code = new int[] {(int)KeyCode.Mouse0},
                CONTROLLER_name = new string[] {"Right Trigger"}
            }
        },
        {
            "AttackBasic", new eevee.config {
                displayName = "AttackBasic",
                KEYBOARD_code = new int[] {(int)KeyCode.Mouse1},
                CONTROLLER_name = new string[] {"Left Trigger"}
            }
        },

        {
            "Jump", new eevee.config {
                displayName = "Jump",
                KEYBOARD_code = new int[] {(int)KeyCode.Space},
                CONTROLLER_name = new string[] {"A"}
            }
        },
        {
            "Slam", new eevee.config {
                displayName = "Slam",
                KEYBOARD_code = new int[] {(int)KeyCode.LeftControl},
                CONTROLLER_name = new string[] {"Left Sick Press"}
            }
        },
        {
            "Dash", new eevee.config {
                displayName = "Dash",
                KEYBOARD_code = new int[] {(int)KeyCode.LeftShift},
                CONTROLLER_name = new string[] {"X"}
            }

        },
        {
            "interact", new eevee.config {
                displayName = "interact",
                KEYBOARD_code = new int[] {(int)KeyCode.E},
                CONTROLLER_name = new string[] {"Right Trigger"}
            }
        },
        {
            "back", new eevee.config {
                displayName = "back",
                KEYBOARD_code = new int[] {(int)KeyCode.Q},
                CONTROLLER_name = new string[] {"B"}
            }
        },

        // camera
        {
            "cameraLeft", new eevee.config {
                displayName = "cameraLeft",
                KEYBOARD_code = new int[] {(int)KeyCode.LeftArrow},
                CONTROLLER_name = new string[] {"Left Stick Up"}
            }
        },
        {
            "cameraRight", new eevee.config {
                displayName = "cameraRight",
                KEYBOARD_code = new int[] {(int)KeyCode.RightArrow},
                CONTROLLER_name = new string[] {"Left Stick Down"}
            }
        },

        // menu
        {
            "pause", new eevee.config {
                displayName = "pause",
                KEYBOARD_code = new int[] {(int)KeyCode.Tab},
                CONTROLLER_name = new string[] {"Select"}
            }
        }
    };

    void Start() {
        Dictionary<string, eevee.config> controls = eevee.Qlock.extractr();

        foreach (string key in config.Keys){
            if (controls.Keys.Contains(key) && overwrite){
                Debug.Log($"overwritng {key}");
                eevee.inject.OverWrite(config[key]);
            } else if (!controls.Keys.Contains(key)){
                eevee.inject.add(config[key]);
            }
        }
    }

    void Update() {
        
    }
}
