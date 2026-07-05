using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using save;

public class gamaController : MonoBehaviour {
    public static gamaController instance;
    private LiftGammaGain gain; // post processing for it

    void Start() {
        GetComponent<Volume>().sharedProfile.TryGet<UnityEngine.Rendering.Universal.LiftGammaGain>(out gain);
        instance = this;

        loadGama();
    }

    public void loadGama() {
        if (gain == null) return;

        float gama = getData.config().gama;
        Debug.Log($"setting gama to {gama}");
        gain.gain.value = new Vector4(gama, gama, gama, gama);
    }
}
