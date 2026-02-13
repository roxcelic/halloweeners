using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneLoader : MonoBehaviour {
    public string toLoad;

    public void open() {
        if (toLoad == null) return;
        SceneManager.LoadScene(toLoad);
    }
}