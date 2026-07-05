using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickDetector : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData){Debug.Log("Clicked on UI Element: " + eventData.pointerCurrentRaycast.gameObject.name);}
}
