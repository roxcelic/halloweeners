using System;

using UnityEngine;
using UnityEngine.EventSystems;

using TMPro;

public sealed class N_PM_clickable : MonoBehaviour, ISerializationCallbackReceiver, IPointerClickHandler {
    [SerializeField]
    private TMP_Text textComponent;
    private Vector2 lastPointerPosition = new Vector2();

    public void OnBeforeSerialize() {
        if (textComponent == null) {
            textComponent = GetComponent<TMP_Text>();
        }
    }

    public void OnAfterDeserialize() { }

    public void OnPointerClick(PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, null);

        if (linkIndex == -1) {
            return;
        }

        var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
        string link = linkInfo.GetLink();

        sys.var.components.pauseMenu().runOption(Int32.Parse(link));
    }

    void Update() {
        Vector2 pointerDelta = (Vector2)Input.mousePosition - lastPointerPosition;
        lastPointerPosition = Input.mousePosition;

        if (pointerDelta.magnitude > 0f) {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, null);
            if (linkIndex == -1) return;

            var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
            string link = linkInfo.GetLink();

            sys.var.components.pauseMenu().hoveredIndex = Int32.Parse(link);
            sys.var.components.pauseMenu().displayText();
        }
    }
}