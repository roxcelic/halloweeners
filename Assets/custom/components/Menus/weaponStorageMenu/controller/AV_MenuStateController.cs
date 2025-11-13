using UnityEngine;

public class AV_MenuStateController : MonoBehaviour {
    [Header("components")]
    public string playerTag = "Player";
    public brain mainBrain;
    public bool interactAble = false;
    public AV_MenuController AV;

    [Header("data")]
    public sys.Text openText = new sys.Text();

    void Update() {
        GameObject target = transform.GetChild(0).gameObject;
        
        if (!target.activeSelf && (!interactAble || GS.live.state.paused || GS.live.state.helped)) return;
        if (eevee.input.Collect("interact", "AVO")) {
            if (AV.inMenu) {
                // AV.closeMenu();
                return;
            }

            bool apply = !target.activeSelf;
            target.SetActive(apply);
            GS.live.state.menu(apply);
        }
    }

    public void closeMenu() {
        GameObject target = transform.GetChild(0).gameObject;
        target.SetActive(false);
        GS.live.state.menu(false);
    }

    #region colliderShit
    void OnTriggerEnter(Collider other) {if (other.gameObject.tag == playerTag) interactAble = true; mainBrain.thought = sys.text.displayKeyButton(openText.localise());}
    void OnTriggerExit(Collider other) {if (other.gameObject.tag == playerTag) interactAble = false; mainBrain.thought = sys.text.displayKeyButton($"i hate you ):");}
    #endregion
}
