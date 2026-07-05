using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class POMO_ExportButton : MonoBehaviour {
    public RenderTexture Tex;

    private string getPath() {return $"{Application.persistentDataPath}/Photos";}

    public void export() {
        Texture2D finalTex = converRT(Tex);

        Directory.CreateDirectory(getPath());
        string url = $"{getPath()}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.png";
        File.WriteAllBytes(url, finalTex.EncodeToPNG());
        Application.OpenURL(url);
    }

    public void openFolder() {
        Directory.CreateDirectory(getPath());
        Application.OpenURL(getPath());
    }

    private Texture2D converRT(RenderTexture tex) {
        Texture2D newTex = new Texture2D(tex.width, tex.height);
        RenderTexture.active = tex;
        newTex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        newTex.Apply();
        return newTex;
        
    }
}