using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/switch weapon")]
public class PM_switchWrapon : PM_Base {
    public override void action(pauseMenuController PMC, string input = "") {
        children = new List<PM_Base>();

        PMC.log("starting search");

        foreach (attack.attackRegistration attack in GS.live.state.registeredAttacks) {
            PM_weapon weapon = ScriptableObject.CreateInstance("PM_weapon") as PM_weapon;
            weapon.attack = attack.attack;
            weapon.name = attack.attack.displayName;

            children.Add(weapon);
        
            PMC.log($"found weapon: {attack.attack.displayName.localise()}");
        }

        PMC.log("finished search");

        PMC.loadMenu(children);
    }
}