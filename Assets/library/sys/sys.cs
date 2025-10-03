using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace sys {
    public class system {

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        // public static void applyRes() {
        //     string[] res = config.read.String("screenRes", "1920x1080").Split("x");

        //     Screen.SetResolution(int.Parse(res[0]), int.Parse(res[1]), true);
        // }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void applyFrameCap() {
            Application.targetFrameRate = 60;
        }
    }

    public class text {
        public static string displayKeyButton(string input) {
            // find text
            string[] words = input.Split(" ");
            List<string> selectedWords = new List<string>();

            foreach(string word in words) {
                if ((word.Length - 1) > 5 && word.Substring(0, 5) == "!key:") {
                    string key = word.Substring(5, word.Length - 5);
                    Dictionary<string, eevee.config> FullConfig = eevee.inject.retrieve().FullConfig;
                    if (!FullConfig.ContainsKey(key)) selectedWords.Add(word);

                    eevee.config selected_input = FullConfig[key];

                    switch(eevee.conf.autoDetect()) {
                        case eevee.inputCL.keyboard: 
                            foreach (int keyCode in selected_input.KEYBOARD_code) selectedWords.Add(((KeyCode)keyCode).ToString());

                            break;
                        case eevee.inputCL.controller: 
                            foreach (string buttonCode in selected_input.CONTROLLER_name) selectedWords.Add(buttonCode);

                            break;
                    }
                } else {
                    selectedWords.Add(word);
                }
            }

            return string.Join(" ", selectedWords);
        }
    }

    public class nockback {
        public static Vector3 calculateNockback(Vector3 explosionPoint, Vector3 playerPos, float explosionForce = 5f) {
            Vector2 originalForce = new Vector2();
            originalForce.x = playerPos.x - explosionPoint.x;
            originalForce.y = playerPos.y - explosionPoint.y;   

            Vector2 forceDirection = new Vector2();
            forceDirection = originalForce * originalForce; 

            Vector2 forcePercent = new Vector2();
            float totalForce = forceDirection.x + forceDirection.y;

            forcePercent.x = Math.Abs(forceDirection.x / totalForce);
                if (originalForce.x < 0) forcePercent.x = forcePercent.x * -1;
            forcePercent.y = Math.Abs(forceDirection.y / totalForce);
                if (originalForce.y < 0) forcePercent.y *= -1;

            return (new Vector3(-forcePercent.x, 1, -forcePercent.y)) * explosionForce;
        }
    }
}