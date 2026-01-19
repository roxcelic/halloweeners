using UnityEngine;

public class brain : MonoBehaviour {
    public enum brainTypes {
        interactable,
        enemy
    }

    public brainTypes brainT;
    public string thought = "";
}