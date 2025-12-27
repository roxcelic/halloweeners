using UnityEngine;

public class POR_MTPC : MonoBehaviour {
    public Transform PlayerCamera;
    public Transform portal;

    [Header("position")]
    private Vector3 startPos;
    public bool Posx = true;
    public bool Posy = true;
    public bool Posz = true;

    [Header("rotation")]
    public Vector3 angelOffset;
    void Start() {
        startPos = transform.position;
    }
    void Update() {
        // rotation
        float ADBPR = Quaternion.Angle(transform.rotation, portal.rotation);
        Quaternion prd = Quaternion.AngleAxis(ADBPR, Vector3.up);
        Vector3 newCD = prd * PlayerCamera.forward;
        // transform.rotation = Quaternion.LookRotation(newCD, Vector3.up);
        transform.eulerAngles = PlayerCamera.eulerAngles;
        
        // position
        transform.position = new Vector3(
            Posx ? PlayerCamera.position.x : startPos.x,
            Posy ? PlayerCamera.position.y : startPos.y,
            Posz ? PlayerCamera.position.z : startPos.z
        );
    }
}
