using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/base")]
public class PM_Base : ScriptableObject {
    [Header("text")]
    new public sys.Text name = new sys.Text();

    [Header("data")]
    public bool essential;
    public bool dev = false;
    public List<PM_Base> children;

    public async virtual void action(pauseMenuController PMC, string input = "") {
        PMC.loadMenu(children);
    }
    
    public virtual void onLoad(pauseMenuController PMC) {} // most will do nothing with this

    /*
        Here i will be like doing stuff :steamhappy:
    */
    #region utils
    public playerController findPlayer() {return GameObject.FindGameObjectsWithTag("Player")[0].transform.GetComponent<playerController>();} // a simple one liner to find the player
    public void updateScreen(pauseMenuController PMC) {PMC.displayText();}
    #endregion
}
