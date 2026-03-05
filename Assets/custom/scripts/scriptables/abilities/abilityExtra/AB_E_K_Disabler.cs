using UnityEngine;

public class AB_E_K_Disabler : enableOnInput {
    [Range(0f, 1f)] public float disableDelay;

    protected override void Update() {
        if (!eevee.input.Check(input) && (AB_kunai.hit == null)) {
            if (destroyOnDisable) {
                sys.utils.waiting.waitForSeconds(() => {
                    Destroy(transform.gameObject);
                }, disableDelay);
                return;
            } sys.utils.waiting.waitForSeconds(() => {
                transform.gameObject.SetActive(false);
            }, disableDelay);
        }
    }
}
