using UnityEngine;

public class MapController : MonoBehaviour {
    [Header("components")]
    public string playerTag = "Player";
    public brain mainBrain;
    public bool interactAble = false;

    [Header("data")]
    public sys.Text openText = new sys.Text();

    void Update() {
        GameObject target = transform.GetChild(0).gameObject;
        
        if (!target.activeSelf && (!interactAble || GS.live.state.paused || GS.live.state.helped)) return;
        
        if (eevee.input.Collect("interact")) {
            bool apply = !target.activeSelf;
            target.SetActive(apply);
            GS.live.state.menu(apply);
        }
    }

    #region colliderShit
    void OnTriggerEnter(Collider other) {if (other.gameObject.tag == playerTag) interactAble = true; mainBrain.thought = sys.text.displayKeyButton(openText.localise());}
    void OnTriggerExit(Collider other) {if (other.gameObject.tag == playerTag) interactAble = false; mainBrain.thought = sys.text.displayKeyButton($"i hate you ):");}
    #endregion
}
