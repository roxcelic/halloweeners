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

    public virtual void action(pauseMenuController PMC, string input = "") {
        PMC.loadMenu(children);
    }
    public virtual void onLoad(pauseMenuController PMC) {} // most will do nothing with this
    public virtual bool active() {return true;} // checks if the thing can be ran
    public virtual void runOnLoad() {} // run code when the thing is loaded

    /*
        Here i will be like doing stuff :steamhappy:
    */
    #region utils
    public playerController findPlayer() {return playerController.mainPlayer;} // a simple one liner to find the player
    public void updateScreen(pauseMenuController PMC) {PMC.displayText();}
    public bool isSelected(pauseMenuController PMC) {return PMC.isSelected(this);}

    // generate a list of items
    public List<PM_Base> generateChildren<T> (List<string> source, Action<string> act) where T : PM_StringInput {
        List<PM_Base> items = new List<PM_Base>();

        foreach (string item in source) {
            var type = typeof(T);
            T obj = (T)ScriptableObject.CreateInstance(type);

            obj.name.overrideName = item;
            obj.data = item;
            obj.act = act;

            items.Add(obj);
        }

        return items;
    }
    #endregion
}
