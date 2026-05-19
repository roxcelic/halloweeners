using UnityEngine;
using UnityEngine.SceneManagement;

public class uiuitels : MonoBehaviour {
    [Header("dynamic")]
    public System.Action dynamicCode;

    public void kill(bool kill = true) {transform.gameObject.SetActive(kill);}
    public void reloadScene() {SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
    public void destroy() {Destroy(transform.gameObject);}
    public void disable() {transform.gameObject.SetActive(false);}
    public void enable() {transform.gameObject.SetActive(true);}
    public void runDynamic() {dynamicCode();}
    public void spawn() {}
    public void playSound() {transform.GetComponent<AudioSource>().Play();}
    public void randomisePos() {transform.position = randomPositionOnCanvas(new Vector2(25f, 25f));}

    #region  idfk ask me later
    Vector2 randomPositionOnCanvas(Vector2 screenMargin) {
        return new Vector2(Random.Range(0 + screenMargin.x, Screen.width - screenMargin.x), Random.Range(0 + screenMargin.y, Screen.height - screenMargin.y));
    }
    #endregion
}
