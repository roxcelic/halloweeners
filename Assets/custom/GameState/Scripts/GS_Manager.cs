using UnityEngine;

public class GS_Manager : MonoBehaviour {
    public GameState GameS;

    void Start() {
        GameS.paused = false;
    
        // load
        GS.live.state = GameS;
    }
}