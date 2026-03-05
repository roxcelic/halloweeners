using UnityEngine;

public class AB_E_K_Disabler : enableOnInput {
    protected override void Update() {
        if (!eevee.input.Check(input) && (AB_kunai.hit == null)) {
            if (destroyOnDisable) {
                Destroy(transform.gameObject);
                return;
            } transform.gameObject.SetActive(false);
        }
    }
}
