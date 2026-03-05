using UnityEngine;

public class enableOnInput : MonoBehaviour {
    [Header("input")]
    public string input = "";

    [Header("config")]
    public bool destroyOnDisable = false;

    protected virtual void Update() {
        if (!eevee.input.Check(input)) {
            if (destroyOnDisable) {
                Destroy(transform.gameObject);
                return;
            } transform.gameObject.SetActive(false);
        }
    }
}