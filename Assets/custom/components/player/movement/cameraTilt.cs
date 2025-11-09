using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class cameraTilt : MonoBehaviour {
    [Header("config")]
    public bool active = false;
    public bool shaking = false;
    public float maxTilt;
    [Range(0f, 10f)] public float leniance;
    [Range(0f, 5f)] public float tiltModifier = 1.25f;
    [Range(0f, 15f)] public float tiltSpeed = 2.5f;

    public bool favourDown = true;
    [Range(0f, 10f)] public float downModifier = 2f;

    [Header("components")]
    new public Transform camera;
    
    private Rigidbody rb;
    
    void Start() {
        rb = transform.GetComponent<Rigidbody>();
    }

    void Update() {
        if (!active) return;

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

    public void shake(float range, int passes = 2) {if (!shaking) StartCoroutine(shaker(range, passes));}

    public IEnumerator shaker(float range, int passes = 2) {
        shaking = true;
        for (int i = 0; i < passes; i++) {
            float targetShake = UnityEngine.Random.Range(-range, range);
            while (Mathf.Abs(camera.eulerAngles.x) - Mathf.Abs(targetShake) > 1) {
                Quaternion target = Quaternion.Euler(0, targetShake, 0);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, target,  Time.deltaTime * tiltSpeed);
                yield return 0;
            }
        }

        while (Mathf.Abs(camera.eulerAngles.x) > 1) {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, target,  Time.deltaTime * tiltSpeed);
            yield return 0;
        }
        shaking = false;
    }
}
