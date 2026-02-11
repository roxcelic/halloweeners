using UnityEngine;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/pause menu/dev/cam Edit")]
public class PM_cameraEdit : PM_Base {
    private float camPos = 0f;

    public async override void action(pauseMenuController PMC, string input = "") {
        camPos = await PMC.getValueSlider(camPos, 10f, 0f); 
        playerController.mainPlayer.camera.localPosition = new Vector3(0f, camPos, -camPos);
    }
}