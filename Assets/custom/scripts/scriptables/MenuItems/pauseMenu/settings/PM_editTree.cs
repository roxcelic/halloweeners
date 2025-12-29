using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/editTree")]
public class PM_editTree : PM_Base {
    public GameObject selectedObj;

    public async override void action(pauseMenuController PMC, string input = "") {
        // save data
        string[] commandData = (await PMC.getText("")).Split(" ");

        AT_base attack = GS.live.state.player.transform.GetComponent<playerController>().D_Attack;

        switch (commandData[0]) {
            case "list": // lists the available objects
                foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) {
                    PMC.log(obj.name, "dev", "green");
                    loopThroughObjects(obj, PMC);
                }

                break;
            case "select": // lets you select an object
                selectedObj = GameObject.Find(commandData[1]);
                if (selectedObj != null) PMC.log($"succesfully selected object {selectedObj.name}", "system", "blue");
                else PMC.log($"unable to find object {commandData[1]}", "system", "blue");

                break;
            case "view": // views your selected object
                if (selectedObj == null) {
                    PMC.log("sorry but you have no object selected to view", "system", "blue");
                    return;
                }

                PMC.log(selectedObj.name, "dev", "green");
                Component[] components = selectedObj.GetComponents(typeof(Component));
                foreach(Component component in components) PMC.log("\t" + component.GetType().ToString(), "dev", "green");

                break;
            case "edit": // lets you edit your selected object
                if (selectedObj == null) {
                    PMC.log("sorry but you have no object selected to view", "system", "blue");
                    return;
                }

                Component selectedComp = selectedObj.GetComponent(commandData[1]);

                if (selectedComp == null) {
                    PMC.log($"sorry but no component of type {commandData[1]} has been found", "system", "blue");
                    return;
                }

                switch(commandData[2]) {
                    case "view":
                        PMC.log("available choices are:", "dev", "green");
                        loopThroughFeilds(selectedComp, PMC);

                        break;
                    case "get":
                        PMC.log($"{selectedComp.GetFieldValue(commandData[3])}", "dev", "green");

                        break;
                    case "set":
                        PMC.log($"{selectedComp.SetFieldValue(commandData[3], commandData[4])}", "dev", "green");

                        break;
                    default:
                        logUseage(PMC);

                        break;
                }

                break;
            default:
                logUseage(PMC);

                break;
        }
        
        // save.getData.save(currentSave);
    }

    private void logUseage(pauseMenuController PMC) {
        string seperator = "\t";

        PMC.log("correct use for this command is: [name]", "system", "blue");
        PMC.log($"{seperator.Multiply(1)}list", "system", "blue");
        PMC.log($"{seperator.Multiply(1)}select", "system", "blue");
        PMC.log($"{seperator.Multiply(2)}object name", "system", "blue");
        PMC.log($"{seperator.Multiply(1)}view", "system", "blue");
        PMC.log($"{seperator.Multiply(1)}tedit", "system", "blue");
        PMC.log($"{seperator.Multiply(2)}[component]", "system", "blue");
        PMC.log($"{seperator.Multiply(3)}[view]", "system", "blue");
        PMC.log($"{seperator.Multiply(3)}[get]", "system", "blue");
        PMC.log($"{seperator.Multiply(3)}[set] [new value]", "system", "blue");
    }

    private void loopThroughFeilds(System.Object obj, pauseMenuController PMC, int loop = 1) {
        var type = obj.GetType();
        string seperator = "\t";
        
        foreach (var sourceProperty in type.GetFields()) {
            if (obj == null) return; 
            PMC.log($"{seperator.Multiply(loop)}{sourceProperty.Name} : {sourceProperty.GetValue(obj)}", "dev", "green");

            List<string> allowedTypes = new List<string>{nameof(attack.attackData), nameof(Vector2), nameof(List<String>)};
            string typeName = sourceProperty.GetValue(obj).GetType().Name;
            if (typeName == null) typeName = "";

            if (loop <= 3 && allowedTypes.Contains(typeName)) {
                loopThroughFeilds(sourceProperty.GetValue(obj), PMC, loop + 1);
            }
        }
    }

    private void loopThroughObjects(GameObject obj, pauseMenuController PMC, int loop = 1) {
        string seperator = "\t";
        
        foreach(Transform child in obj.transform) {
            PMC.log($"{seperator.Multiply(loop)}{child.gameObject.name}", "dev", "green");

            if (loop <= 3) {
                loopThroughObjects(child.gameObject, PMC, loop + 1);
            }
        }
    }
}
