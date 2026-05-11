using UnityEngine;
using ext;
public class noclipcontroller : MonoBehaviour {

    [Header("conf")]
    [Range(0, 25f)] public float distance = 0f;
    public Transform lookat;

    void Update() {
        transform.noClip(null, 0.2f);

        // look at a target
        if (lookat != null) transform.LookAt(lookat);
        
        // lock the allowed distance to move
        if (distance > 0) {
            transform.position = Vector3.Lerp(transform.position, new Vector3 (
                Mathf.Clamp(transform.position.x, -distance, distance),    
                Mathf.Clamp(transform.position.y, -distance, distance),    
                Mathf.Clamp(transform.position.z, -distance, distance)
            ), Time.deltaTime * 5f);
        }
    }
}
