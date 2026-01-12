using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using save;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/volume")]
public class PM_Volume : PM_Base {

    public volumeController.volumeType type;

    public async override void action(pauseMenuController PMC, string input = "") {
        fullConfig conf = getData.config();

        switch (type) {
            case volumeController.volumeType.music:
                conf.volume_music = await PMC.getValueSlider(conf.volume_music, 20f, -80f);
                break;
            case volumeController.volumeType.sfx:
                conf.volume_sfx = await PMC.getValueSlider(conf.volume_sfx, 20f, -80f);
                break;
            case volumeController.volumeType.master: default: 
                conf.volume_master = await PMC.getValueSlider(conf.volume_master, 20f, -80f);
                break;
        }

        volumeController.instance.updateVolumes();

        getData.saveConfig(conf);
    }
}
