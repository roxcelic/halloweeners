using UnityEngine;

using System;
using System.Collections;       
using System.Collections.Generic;

class POR_Connector : MonoBehaviour {
    public POR_Connector connectedPortal;
    public RenderTexture selfDisplay;

    [Header("portal")]
    public POR_BASE portalBase;
    public POR_BR_CAM portalCameraComponent;
    
    [Header("components")]
    public Camera portalCamera;
    public MeshRenderer display;
    public Transform reciever;

    [Header("dev")]
    public Texture2D devTex;
    public Material view;

    void OnEnable() {
        StartCoroutine(setupConnection());
    } 

    public IEnumerator setupConnection() {
        Debug.Log("portal setup");
        selfDisplay = new RenderTexture(sys.var.screen.width, sys.var.screen.height, 16, RenderTextureFormat.ARGB32);
        portalCamera.targetTexture = selfDisplay;

        Debug.Log($"connected: {connectedPortal.selfDisplay} :: null = {connectedPortal.selfDisplay != null}");
        yield return new WaitUntil(() => connectedPortal.selfDisplay != null);
        while(connectedPortal.selfDisplay == null) {
            Debug.Log("IM NULL");
            yield return null;
        }
        Debug.Log("Im Not Null!!!!!!!!!!!!!!!!!!!!!");

        Material material = new Material(Shader.Find("Unlit/ScreenCutoutShader"));
        material.SetTexture("_MainTex", devTex == null ? connectedPortal.selfDisplay : devTex);
        display.material = material;
        view = material;
        Debug.Log($"bleh: {material} (did it say material)");

        portalBase.reciever = connectedPortal.reciever;

        portalCameraComponent.otherPortal = connectedPortal.transform;

    }
}