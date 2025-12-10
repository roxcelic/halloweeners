using UnityEngine;

public class activeWithDev : MonoBehaviour {
    void OnEnable() {
        transform.gameObject.SetActive(save.utils.getDev());        
    }
}