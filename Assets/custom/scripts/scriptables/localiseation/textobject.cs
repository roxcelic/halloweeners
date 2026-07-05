using UnityEngine;

[CreateAssetMenu(fileName = "text", menuName = "text/text")]
public class textobject : ScriptableObject {
    [TextArea]
    public string English;
}
