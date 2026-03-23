using UnityEngine;

public class AB_E_K_Disabler : enableOnInput {
    [Range(0f, 1f)] public float disableDelay;
    private bool startedWait = false;

    protected override void Update() {
        if (startedWait) return;

        if (!eevee.input.Check(input) && (AB_kunai.hit == null)) {
            startedWait = true;

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
