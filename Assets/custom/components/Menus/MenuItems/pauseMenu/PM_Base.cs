using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/base")]
public class PM_Base : ScriptableObject {
    new public string name;
    public bool essential;
    public bool dev = false;
    public List<PM_Base> children;

    public virtual void action(pauseMenuController PMC) {
        PMC.loadMenu(children);
    }

    public virtual void onLoad(pauseMenuController PMC) {} // most will do nothing with this

    /*
        Here i will be like doing stuff :steamhappy:
    */
    #region utils
    public playerController findPlayer() {return GameObject.FindGameObjectsWithTag("Player")[0].transform.GetComponent<playerController>();} // a simple one liner to find the player
    
    #endregion
}
