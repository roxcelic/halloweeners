using UnityEngine;

public class basicLevelLoader : MonoBehaviour {
    // basic var
    public string playerTag = "Player";
    private LoadingScreen player;
    public bool customMusic = true;

    void Start() {
        player = GameObject.FindGameObjectsWithTag(playerTag)[0].transform.GetComponent<LoadingScreen>();        
        player.completion = 100;
        musicLib.var.customMusicAccess = customMusic;
    }
}
