using UnityEngine;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/delete save")]
public class PM_DeleteSave : PM_Base {
    public async override void action(pauseMenuController PMC, string input = "") {
        PMC.question(() => {
            save.getData.save(new save.saveData());
            PMC.reset();
        });
    }
}
