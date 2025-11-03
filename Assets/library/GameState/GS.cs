using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace GS {
    public static class live {
        static live () {
            GS.live.state = Resources.Load<GameState>("GameState");
        }
        static public GameState state = null;
    }
}