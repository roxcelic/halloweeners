using UnityEngine;

public class deactiveateElemment : MonoBehaviour {
    public GameObject target;

    public void close() {
        if (target == null) transform.gameObject.SetActive(false);
        else target.SetActive(false);
    }
}