using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AnimateOnHover : MonoBehaviour {
    // variables
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;
    [SerializeField] RectTransform canvasRect;

    [SerializeField] Animator anim;
    
    void Start() {
        anim = transform.GetComponent<Animator>();
        if (anim == null) Debug.Log("unable to get the animator");
    }

    void Update() {
        if (anim == null) return;

        PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        // turn into game objects
        List<GameObject> convertedResults = new List<GameObject>();
        foreach(RaycastResult res in results) convertedResults.Add(res.gameObject); 
        
        // set val
        anim.speed = convertedResults.Contains(transform.gameObject) ? 1 : 0;
    }
}
