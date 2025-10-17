using UnityEngine;
using UnityEngine.SceneManagement;

public class uiuitels : MonoBehaviour {
    public void kill(bool kill = true) {transform.gameObject.SetActive(kill);}
    public void reloadScene() {SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
    public void destroy() {Destroy(transform.gameObject);}
}
