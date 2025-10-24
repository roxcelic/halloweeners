using UnityEngine;

public class basicLevelLoader : MonoBehaviour {
    // basic var
    public string playerTag = "Player";
    private LoadingScreen player;

    void Start() {
        player = GameObject.FindGameObjectsWithTag(playerTag)[0].transform.GetComponent<LoadingScreen>();        
        player.completion = 100;
    }
}
