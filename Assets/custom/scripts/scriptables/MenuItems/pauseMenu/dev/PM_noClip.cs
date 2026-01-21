using UnityEngine;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/noClip")]
public class PM_noClip : PM_Base {
    public async override void action(pauseMenuController PMC, string input = "") {
        playerController.mainPlayer.noclip = !playerController.mainPlayer.noclip;
    }
}