using UnityEngine;

using TMPro;

public class IMP_RunCommand : MonoBehaviour {
    public enum inputType { button }

    [Header("config")]
    public IMP_controller menuController;
    public inputType input;
    public PM_Base command;

    /*

    */
    public void run() {
        switch (input) {
            case inputType.button: default:
                command.newAction(menuController);

            break;
        }
    }
}