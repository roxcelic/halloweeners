using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class lineRendererClip : MonoBehaviour {
    LineRenderer LR;

    [Header("start")]
    public Transform start;
    public bool startAtPlayer = false;
    public bool startAtSelf = false;
    public Vector3 startOffset;

    [Header("end")]
    public Transform end;
    public bool endAtPlayer = false;
    public bool endAtSelf = false;
    public Vector3 endOffset;

    void Start() {LR = transform.GetComponent<LineRenderer>();}

    void Update() {
        LR.SetPosition(0, (startAtPlayer ? playerController.mainPlayer.transform.position : startAtSelf ?  transform.position : start.position) + startOffset);
        LR.SetPosition(1, (endAtPlayer ? playerController.mainPlayer.transform.position : endAtSelf ? transform.position : end.position) + endOffset);
    }
}
