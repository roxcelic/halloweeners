using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/string input")]
public class PM_StringInput : PM_Base {
    public string data = "";
    public Action<string> act = (string input) => {Debug.Log("\"\"");};

    public override void action(pauseMenuController PMC, string input = "") {act(data);}
}
