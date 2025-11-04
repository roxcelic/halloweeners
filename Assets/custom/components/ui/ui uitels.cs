using UnityEngine;
using UnityEngine.SceneManagement;

public class uiuitels : MonoBehaviour {
    public System.Action dynamicCode;

    public void kill(bool kill = true) {transform.gameObject.SetActive(kill);}
    public void reloadScene() {SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
    public void destroy() {Destroy(transform.gameObject);}
    public void disable() {transform.gameObject.SetActive(false);}
    public void enable() {transform.gameObject.SetActive(true);}
    public void runDynamic() {dynamicCode();}
}
