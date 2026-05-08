using UnityEngine;

public class DeactivateOnEnemyDeath : MonoBehaviour {
    public EN_base target;

    void Update() {
        if(target != null) transform.gameObject.SetActive(!target.dead);
    }
}