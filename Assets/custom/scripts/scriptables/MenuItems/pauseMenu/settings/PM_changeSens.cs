using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/change sense")]
public class PM_changeSens : PM_Base {
    public sys.Text senseSetMessage = new sys.Text();

    public async override void action(pauseMenuController PMC, string input = "") {
        fullConfig conf = getData.config();
        conf.sense = await PMC.getValueSlider(conf.sense, 2f, 0.25f); 
        getData.saveConfig(conf);
        PMC.log(senseSetMessage.localise() + conf.sense.ToString(), "", "blue", false);
    }
}
