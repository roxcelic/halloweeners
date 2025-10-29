using UnityEngine;

using TMPro;

public class activateOnStart : MonoBehaviour {
    void Start() {transform.GetComponent<TMP_InputField>().ActivateInputField();}
}