using UnityEngine;

public class GS_Manager : MonoBehaviour {
    public GameState GameS;
    public GameObject player;

    void Start() {
        GameS.paused = false;
        GameS.loaded = false;
        GameS.player = player;
    
        // load
        GS.live.state = GameS;
    }
}