using UnityEngine;

public class MS_matchSpeed : MonoBehaviour {
    public Transform targetUI;
    public float minSpeed = 20f;
    [Range(0, 2f)] public float multiplier = 0.5f;

    [Header("comp")]
    public Rigidbody RB;
    public CanvasGroup CG;
    public Animator Anim;

    [Header("fov stuff")]
    private float baseFov;
    [Range(0, 50f)] public float fovRange = 10f;
    public Camera cam;

    void Start() {
        RB = transform.GetComponent<Rigidbody>();
        CG = targetUI.GetComponent<CanvasGroup>();
        Anim = targetUI.GetComponent<Animator>();

        if (cam != null) baseFov = cam.fieldOfView;
    }

    void Update() {
        float mod = (RB.linearVelocity.magnitude - minSpeed) * multiplier;

        CG.alpha = Mathf.Lerp(CG.alpha, (
            RB.linearVelocity.magnitude < minSpeed
        ) ? 0 : 1, Time.deltaTime * 15f);

        Anim.speed = Mathf.Clamp(mod, 0, Mathf.Infinity);

        if (cam != null) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFov + Mathf.Clamp(mod, 0, fovRange), Time.deltaTime * 5f);
    }

}