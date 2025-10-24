using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject {
    public bool paused = false;

    public void pause(bool pauseSet) {
        Time.timeScale = pauseSet ? 0 : 1;
        paused = pauseSet;

        Cursor.lockState = pauseSet ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pauseSet;
    }
}
