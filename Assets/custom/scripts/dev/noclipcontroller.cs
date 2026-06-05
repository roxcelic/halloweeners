using UnityEngine;
using ext;
public class noclipcontroller : MonoBehaviour {

    [Header("conf")]
    [Range(0, 25f)] public float distance = 0f;
    public Transform lookat;
    public bool freeCam = false;

    void Update() {
        transform.noClip(null, 0.2f, false);

        // look at a target
        if (lookat != null && !freeCam) transform.LookAt(lookat);
        
        // lock the allowed distance to move
        if (distance > 0 && !freeCam) {
            transform.position = Vector3.Lerp(transform.position, new Vector3 (
                Mathf.Clamp(transform.position.x, -distance, distance),    
                Mathf.Clamp(transform.position.y, -distance, distance),    
                Mathf.Clamp(transform.position.z, -distance, distance)
            ), Time.deltaTime * 5f);
        }

        if (freeCam) {
            Vector3 rotateVec = new Vector3();

            if (eevee.input.Collect("cameraLeft", "POMO")) rotateVec += new Vector3(0, -5, 0);
            if (eevee.input.Collect("cameraRight", "POMO")) rotateVec += new Vector3(0, 5, 0);
            if (eevee.input.Collect("cameraUp", "POMO")) rotateVec += new Vector3(-5, 0, 0);
            if (eevee.input.Collect("cameraDown", "POMO")) rotateVec += new Vector3(5, 0, 0);

            transform.Rotate(rotateVec * 15f * Time.fixedDeltaTime);
        }
    }
}
