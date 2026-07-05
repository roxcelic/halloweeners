using UnityEngine;

public class brain : MonoBehaviour {
    public enum brainTypes {
        interactable,
        enemy,
        empty
    }

    public brainTypes brainT;
    public string thought = "";

    public System.Action<playerController> onInteract = null;
}