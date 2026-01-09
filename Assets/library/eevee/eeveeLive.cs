using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eeveeLive {
    // a simple util to preload the controls
    public class util {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void preload() {
            // Debug.Log("pre loading eevee inputs...");
            // eevee.inject.Parasite();
            // Dictionary<string, eevee.config> controls = eevee.Qlock.extractr();
            // foreach (string key in controls.Keys){Debug.Log($"pre loaded {key}");}
        }

        public static void reset() {
            eevee.inject.install(var.config);
        }
    }   

    public class var {
        public static Dictionary<string, eevee.config> config = new Dictionary<string, eevee.config>() {
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
                "Ability", new eevee.config {
                    displayName = "Ability",
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
                    CONTROLLER_name = new string[] {"B"}
                }
            },
            {
                "Dash", new eevee.config {
                    displayName = "Dash",
                    KEYBOARD_code = new int[] {(int)KeyCode.LeftShift},
                    CONTROLLER_name = new string[] {"Y"}
                }

            },
            {
                "interact", new eevee.config {
                    displayName = "interact",
                    KEYBOARD_code = new int[] {(int)KeyCode.E},
                    CONTROLLER_name = new string[] {"X"}
                }
            },
            {
                "back", new eevee.config {
                    displayName = "back",
                    KEYBOARD_code = new int[] {(int)KeyCode.Q},
                    CONTROLLER_name = new string[] {"B"}
                }
            },
            {
                "special", new eevee.config {
                    displayName = "special",
                    KEYBOARD_code = new int[] {(int)KeyCode.Space},
                    CONTROLLER_name = new string[] {"Y"}
                }
            },

            // camera
            {
                "cameraLeft", new eevee.config {
                    displayName = "cameraLeft",
                    KEYBOARD_code = new int[] {(int)KeyCode.LeftArrow},
                    CONTROLLER_name = new string[] {"Right Stick Left"}
                }
            },
            {
                "cameraRight", new eevee.config {
                    displayName = "cameraRight",
                    KEYBOARD_code = new int[] {(int)KeyCode.RightArrow},
                    CONTROLLER_name = new string[] {"Right Stick Right"}
                }
            },

            // menu
            {
                "Pause", new eevee.config {
                    displayName = "Pause",
                    KEYBOARD_code = new int[] {(int)KeyCode.Tab},
                    CONTROLLER_name = new string[] {"Select"}
                }
            }
        };
    }
}