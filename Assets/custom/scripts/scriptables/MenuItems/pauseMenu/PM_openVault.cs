using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/open Vault")]
public class PM_openVault : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {

        // this was my attempt at preventing the transition if mid animation
        // if (PMC.anim.GetCurrentAnimatorClipInfo(0)[0].clip.normalizedTime > 1f) return;

        switch(PMC.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name) {
            case "close":
                PMC.anim.Play("open");

                break;
            case "open":
                PMC.anim.Play("color");

                break;
            case "color":
                PMC.anim.Play("close");

                break;
        }
    }
}
