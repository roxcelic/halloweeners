using System;

using UnityEngine;
using UnityEngine.EventSystems;

using TMPro;

public sealed class N_PM_clickable : MonoBehaviour, ISerializationCallbackReceiver, IPointerClickHandler {
    [SerializeField]
    private TMP_Text textComponent;

    public void OnBeforeSerialize() {
        if (textComponent == null) {
            textComponent = GetComponent<TMP_Text>();
        }
    }

    public void OnAfterDeserialize() { }

    public void OnPointerClick(PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, null);

        if (linkIndex == -1)
        {
            return;
        }

        var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
        string link = linkInfo.GetLink();

        sys.var.components.pauseMenu().runOption(Int32.Parse(link));
    }

    public void OnPointerMove(PointerEventData eventData){
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, null);

        if (linkIndex == -1)
        {
            return;
        }

        var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
        string link = linkInfo.GetLink();

        Debug.Log($"hovering: {link}");
        sys.var.components.pauseMenu().selectedIndex = Int32.Parse(link);
    }
}