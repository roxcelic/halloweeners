using UnityEngine;

using System;
using System.Collections;

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
}