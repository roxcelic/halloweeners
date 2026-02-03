using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/practice/cutomisation")]
public class PM_practiceOptions : PM_Base {
    // an "actual" name for the item
    public sys.Text actualName = new sys.Text();

    [Serializable]
    public class itemOption {
        public itemOption(int refrence = 0, sys.Text name = null, string refrenceName = "") {
            this.refrence = refrence;
            this.refrenceName = refrenceName;
            this.name = name;
        }

        public int refrence;
        public sys.Text name;
        public string refrenceName;
    }
    
    // options
    public enum optionsType {
        difficulty,
        map
    }
    public optionsType options;

    // data
    public List<itemOption> values = new List<itemOption>();
    public itemOption selected;

    public override void runOnLoad() {
        selected = new itemOption();
        switchSelected();
    }

    public override void action(pauseMenuController PMC, string input = "") {
        switchSelected();
    }

    private void switchSelected() {
        // catch to see if its empty
        if (values.Count == 0) {
            Debug.Log("options are empty");
            return;
        }

        if (selected == new itemOption()) selected = values[0];
        else {
            int foundIndex = values.FindIndex(a => a == selected) + 1;
            if (foundIndex >= values.Count) foundIndex = 0;

            selected = values[foundIndex];
            Debug.Log($"found index: {foundIndex} which is {values[foundIndex]}");
            Debug.Log(selected);
        }

        updateName();
    }

    private void updateName() {
        name.overrideName = $"{actualName.localise()}: {selected.name.localise()}";
    }
}
