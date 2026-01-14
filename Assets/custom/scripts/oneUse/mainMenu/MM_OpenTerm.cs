using UnityEngine;

public class MM_OpenTerm : MonoBehaviour {
    public MM_Term term;

    void OnTriggerEnter() {
        term.open();
    }
}