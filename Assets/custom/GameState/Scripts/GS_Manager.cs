using UnityEngine;

public class GS_Manager : MonoBehaviour {
    public GameState GameS;
    public GameObject player;

    void Start() {
        GS.live.state.paused = false;
        GS.live.state.helped = false;
        GS.live.state.menued = false;
        GS.live.state.loaded = false;
        GS.live.state.player = player;
    }
}