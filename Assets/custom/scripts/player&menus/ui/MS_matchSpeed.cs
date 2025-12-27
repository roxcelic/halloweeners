using UnityEngine;

public class MS_matchSpeed : MonoBehaviour {
    public Transform targetUI;
    public float minSpeed = 20f;
    [Range(0, 2f)] public float multiplier = 0.5f;

    [Header("comp")]
    public Rigidbody RB;
    public CanvasGroup CG;
    public Animator Anim;

    void Start() {
        RB = transform.GetComponent<Rigidbody>();
        CG = targetUI.GetComponent<CanvasGroup>();
        Anim = targetUI.GetComponent<Animator>();
    }

    void Update() {
        CG.alpha = Mathf.Lerp(CG.alpha, (
            RB.linearVelocity.magnitude < minSpeed
        ) ? 0 : 1, Time.deltaTime * 15f);

        Anim.speed = (RB.linearVelocity.magnitude - minSpeed) * multiplier;
    }

}