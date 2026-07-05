using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class POMO_ImageDisplay_Cont : MonoBehaviour {
    public static POMO_ImageDisplay_Cont self;

    [Header("conf")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("prefabs")]
    public GameObject row;
    public GameObject item;

    [Header("comp")]
    public Transform root;
    private CanvasGroup cg;
    private Animator anim;

    // var
    public System.Action<Sprite> loadedAction = null;

    void Start() {
        cg = transform.GetComponent<CanvasGroup>();
        anim = transform.GetComponent<Animator>();

        loadedAction = (Sprite image) => {Debug.Log($"no value loaded into the action");};
        self = this;

        StartCoroutine(loadLocalSprites());

        enable(false);
    }

    public void compile() {
        int safeCounter = 0;
        List<GameObject> currentRow = new List<GameObject>();
        currentRow.Add(GameObject.Instantiate(row, new Vector3(), Quaternion.identity));
        currentRow[currentRow.Count - 1].transform.parent = root;
        currentRow[currentRow.Count - 1].transform.localScale = new Vector3(1, 1, 1);
        
        for (int i = 0; i < sprites.Count; i++) {
            safeCounter++;
            GameObject genSprite = createItem(sprites[i]);
            genSprite.transform.parent = currentRow[currentRow.Count - 1].transform;
            
            // genSprite.transform.scale = new Vector3(1, 1, 1);
            genSprite.transform.localScale = new Vector3(1, 1, 1);
            
            if (safeCounter >= 5) {
                safeCounter = 0;
                currentRow.Add(GameObject.Instantiate(row, new Vector3(), Quaternion.identity));
                currentRow[currentRow.Count - 1].transform.parent = root;
                currentRow[currentRow.Count - 1].transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    /// <summery> makes an image display </summery>
    public GameObject createItem(Sprite sprite) {
        GameObject spawned = GameObject.Instantiate(item, new Vector3(), Quaternion.identity);
        POMO_ImageDisplay display = spawned.transform.GetComponent<POMO_ImageDisplay>();

        display.display.sprite = sprite;
        display.sprite = sprite;
        return spawned;
    }

    /// <summery> loads local images </summery>
    IEnumerator loadLocalSprites() {
        List<Sprite> localSprites = new List<Sprite>();

        string dirPath = $"{Application.persistentDataPath}/Images/";
        Directory.CreateDirectory(dirPath);
        
        string[] files = Directory.GetFiles(dirPath);

        foreach (string file in files) {
            string[] splitFile = file.Split("/");
            string path = splitFile[splitFile.Length - 1];


            bool continuer = path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg");

            if (continuer) using (UnityWebRequest www = UnityWebRequestTexture.GetTexture($"file:///{file}")) {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
                    Debug.LogError(www.error);
                } else {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    Sprite finalText = null;

                    if (texture != null) finalText = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    if (finalText != null) sprites.Add(finalText);
                }
            }
        }

        compile();
    }

    #region utils
    public void enable(bool value = true) {
        cg.alpha = value ? 1 : 0;
        cg.interactable = value;
        cg.blocksRaycasts = value;

        if (value) anim.Play("open");
    }
    #endregion
}