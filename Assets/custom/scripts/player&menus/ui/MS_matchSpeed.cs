using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using save;

public class MS_matchSpeed : MonoBehaviour {
    public Transform targetUI;
    public float minSpeed = 20f;
    [Range(0, 2f)] public float multiplier = 0.5f;

    [Header("comp")]
    public Rigidbody RB;
    public CanvasGroup CG;
    public Animator Anim;

    [Header("fov stuff")]
    [Range(0, 50f)] public float fovRange = 10f;
    public List<Camera> cams;

    void Start() {
        RB = transform.GetComponent<Rigidbody>();
        CG = targetUI.GetComponent<CanvasGroup>();
        Anim = targetUI.GetComponent<Animator>();
    }

    void Update() {
        float mod = (RB.linearVelocity.magnitude - minSpeed) * multiplier;

        CG.alpha = Mathf.Lerp(CG.alpha, Mathf.Clamp((
            RB.linearVelocity.magnitude < minSpeed
        ) ? 0 : 1, getData.viewSave().lockSpeedDisplay ? 1 : 0, 1), Time.deltaTime * 15f);

        Anim.speed = Mathf.Clamp(mod, 0.25f, Mathf.Infinity);

        if (cams.Count > 0) foreach (Camera cam in cams) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, getData.config().fov + Mathf.Clamp(mod, 0, fovRange), Time.fixedDeltaTime * 5f);
    }

}