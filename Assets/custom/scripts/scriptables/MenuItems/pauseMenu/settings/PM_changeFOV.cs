using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/change fov")]
public class PM_changeFov : PM_Base {
    public sys.Text fovSetMessage = new sys.Text();

    public async override void action(pauseMenuController PMC, string input = "") {
        fullConfig conf = getData.config();
        conf.fov = await PMC.getValueSlider(conf.fov, 150f, 10f); 
        getData.saveConfig(conf);
        PMC.log(fovSetMessage.localise() + conf.fov.ToString(), "", "blue", false);
    }
}
