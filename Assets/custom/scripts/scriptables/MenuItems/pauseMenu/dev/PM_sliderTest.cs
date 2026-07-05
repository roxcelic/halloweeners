using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/slider test")]
public class PM_sliderTest : PM_Base {
    public async override void action(pauseMenuController PMC, string input = "") {
        PMC.log($"activating slider test","","blue", false);
        PMC.log($"slider rest finished with a result of {await PMC.getValueSlider(10, 20)}","","blue", false);
    }
}