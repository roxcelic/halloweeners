using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/settings/SaveAttribute")]
public class PM_SaveAttribute : PM_Base {
    public enum searchType {
        save,
        player,
        attack,
        baseAttack
    }

    public searchType method;

    public override void action(pauseMenuController PMC, string input = "") {
        // save data
        string[] commandData = input.Split(" ");

        switch (method) {
            case searchType.save:
                save.saveData currentSave = save.getData.viewSave();

                switch (commandData[0]) {
                    case "view":
                        PMC.log("available choices are:", "dev", "green");
                        loopThroughFeilds(currentSave, PMC);

                        break;
                    case "get":
                        Debug.Log(currentSave.GetFieldValue(commandData[1]));
                        PMC.log($"{currentSave.GetFieldValue(commandData[1])}", "dev", "green");

                        break;
                    case "set":
                        PMC.log($"{currentSave.SetFieldValue(commandData[1], commandData[2])}", "dev", "green");

                        break;
                    default:
                        PMC.log($"currect use for this command is '[name] [view/get/set] [property name] [new value]'", "system", "blue");

                        break;
                }
                save.getData.save(currentSave);

                break;
            case searchType.player:
                playerController player = GS.live.state.player.transform.GetComponent<playerController>();

                switch (commandData[0]) {
                    case "view":
                        PMC.log("available choices are:", "dev", "green");
                        loopThroughFeilds(player, PMC);

                        break;
                    case "get":
                        PMC.log($"{player.GetFieldValue(commandData[1])}", "dev", "green");

                        break;
                    case "set":
                        PMC.log($"{player.SetFieldValue(commandData[1], commandData[2])}", "dev", "green");

                        break;
                    default:
                        PMC.log($"currect use for this command is '[name] [view/get/set] [property name] [new value]'", "system", "blue");

                        break;
                }
                
                // save.getData.save(currentSave);

                break;
            case searchType.attack:
                AT_base baseAttack = GS.live.state.player.transform.GetComponent<playerController>().attack;

                switch (commandData[0]) {
                    case "view":
                        PMC.log("available choices are:", "dev", "green");
                        loopThroughFeilds(baseAttack, PMC);

                        break;
                    case "get":
                        PMC.log($"{baseAttack.GetFieldValue(commandData[1])}", "dev", "green");

                        break;
                    case "set":
                        PMC.log($"{baseAttack.SetFieldValue(commandData[1], commandData[2])}", "dev", "green");

                        break;
                    default:
                        PMC.log($"currect use for this command is '[name] [view/get/set] [property name] [new value]'", "system", "blue");

                        break;
                }
                
                // save.getData.save(currentSave);

                break;
            case searchType.baseAttack:
                AT_base attack = GS.live.state.player.transform.GetComponent<playerController>().D_Attack;

                switch (commandData[0]) {
                    case "view":
                        PMC.log("available choices are:", "dev", "green");
                        loopThroughFeilds(attack, PMC);

                        break;
                    case "get":
                        PMC.log($"{attack.GetFieldValue(commandData[1])}", "dev", "green");

                        break;
                    case "set":
                        PMC.log($"{attack.SetFieldValue(commandData[1], commandData[2])}", "dev", "green");

                        break;
                    default:
                        PMC.log($"currect use for this command is '[name] [view/get/set] [property name] [new value]'", "system", "blue");

                        break;
                }
                
                // save.getData.save(currentSave);

                break;
        }
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
}
