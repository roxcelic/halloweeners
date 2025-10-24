using UnityEngine;

public class hudDisplay : MonoBehaviour {
    [Header("components")]
    public GameObject textItem;
    public Vector2 screenMargin = new Vector2(15f, 15f);
    
    /*
        This main function of this script
    */
    public void displayText(string text, Color color, bool randomRotation = true, float rotateMax = 45f, bool center = false ) {
        Vector2 spawnPos = center ? new Vector2(Screen.width / 2, Screen.height / 2) : randomPositionOnCanvas();
        Quaternion rotation = randomRotation ? Quaternion.Euler(new Vector3(0, 0, Random.Range(-rotateMax, rotateMax))) : Quaternion.identity;

        GameObject currentText = Instantiate(textItem, spawnPos, rotation, transform);
        currentText.transform.GetComponent<TMPro.TMP_Text>().text = text;
        currentText.transform.GetComponent<TMPro.TMP_Text>().color = color;
    }

    /*
        utils
    */
    Vector2 randomPositionOnCanvas() {
        return new Vector2(Random.Range(0 + screenMargin.x, Screen.width - screenMargin.x), Random.Range(0 + screenMargin.y, Screen.height - screenMargin.y));
    }

}