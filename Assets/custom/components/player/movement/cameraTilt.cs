using UnityEngine;

public class cameraTilt : MonoBehaviour {
    [Header("config")]
    public float maxTilt;
    [Range(0f, 10f)] public float leniance;
    [Range(0f, 5f)] public float tiltModifier = 1.25f;
    [Range(0f, 15f)] public float tiltSpeed = 2.5f;

    public bool favourDown = true;
    [Range(0f, 10f)] public float downModifier = 2f;

    [Header("components")]
    public Transform camera;
    
    private Rigidbody rb;
    
    void Start() {
        rb = transform.GetComponent<Rigidbody>();
    }

    void Update() {
        float yVel = rb.linearVelocity.y;
        float tilt = 0f;

        if (Mathf.Abs(yVel) > leniance) {
            if (yVel > 0) {
                // positive
                tilt = calcTilt(yVel - leniance);
            } else {
                // negative
                tilt = calcTilt(yVel + leniance);
            }
        }

        if (favourDown && tilt > 0) tilt *= downModifier;

        Quaternion target = Quaternion.Euler(tilt, 0, 0);
        camera.localRotation = Quaternion.Slerp(camera.localRotation, target,  Time.deltaTime * tiltSpeed);
    }

    private float calcTilt(float velocity) {
        velocity *= tiltModifier;

        return Mathf.Clamp(velocity, -maxTilt, maxTilt);
    }
}
