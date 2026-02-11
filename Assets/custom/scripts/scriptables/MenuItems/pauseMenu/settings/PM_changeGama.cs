using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/change gama")]
public class PM_changeGama : PM_Base {
    public sys.Text gamaSetMessage = new sys.Text();

    public async override void action(pauseMenuController PMC, string input = "") {
        fullConfig conf = getData.config();
        conf.gama = await PMC.getValueSlider(conf.gama, 0f, -1f); 
        getData.saveConfig(conf);
        if (gamaController.instance != null) {
            gamaController.instance.loadGama();
            PMC.log(gamaSetMessage.localise() + conf.gama.ToString(), "", "blue", false);
        }
    }
}
