using UnityEngine;

public class interactableMenu : MonoBehaviour {
    [Header("components")]
    public string playerTag = "Player";
    public brain mainBrain;
    public bool interactAble = false;

    [Header("data")]
    public sys.Text openText = new sys.Text();

    protected virtual void Update() {
        GameObject target = transform.GetChild(0).gameObject;

        if (!GS.live.state.menued && target.activeSelf) target.SetActive(false);
        else if (!target.activeSelf && (!interactAble || GS.live.state.paused || GS.live.state.helped)) return;
        
        if (eevee.input.Collect("interact", transform.gameObject.name)) {
            bool apply = !target.activeSelf;
            target.SetActive(apply);
            GS.live.state.menu(apply);
        }
    }

    #region colliderShit
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != playerTag) return;
        interactAble = true; 
        mainBrain.thought = sys.text.displayKeyButton(openText.localise());
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag != playerTag) return;
        interactAble = false; 
        mainBrain.thought = sys.text.displayKeyButton($"i hate you ):");
    }
    #endregion
}
