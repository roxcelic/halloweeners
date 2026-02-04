using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using musicLib;

/// <summery> a library for cutscene data, not much too it </summery>
namespace cutscene {
    [System.Serializable]
    public class cutsceneItem {
        public List<Sprite> sprites;

        [Header("text")]
        public bool displayText = false;
        public sys.Text text = new sys.Text();
        
        [Header("song")]
        public bool changeSong = false;
        public string songName = "";

        [Header("color")]
        public bool changeColor = false;
        public Color newColor;

        [Header("position")]
        public bool changePlayerPos = false;
        public Vector3 newPos = new Vector3();

        [Header("audio")]
        public bool playSound = false;
        public AudioClip sound;

        [Header("speed")]
        [Range(0.05f, 1f)] public float textSpeed = 0.1f;
        [Range(0.25f, 4f)] public float imageFadeSpeed = 1f;
        [Range(0.25f, 4f)] public float spriteChangeSpeed = 1f;

        public cutsceneItem(){}
    }

    public static class types {
        public enum startType {
            start,
            enable,
            trigger
        }
    }
}

public class cutsceneController : MonoBehaviour {
    [Header("config")]
    public cutscene.types.startType startOn = cutscene.types.startType.start;
    public bool freezePlayer = true;

    [Header("data")]
    public List<cutscene.cutsceneItem> content = new List<cutscene.cutsceneItem>();
    private bool started = false;

    [Header("components")]
    // Image
    public Image imageDisplay;
    public CanvasGroup imageGroup;
    public CanvasGroup backgroundGroup;
    
    // Text
    public GameObject textContainer;
    public TMP_Text textDisplay;
    public GameObject finishedText;

    // audio
    private AudioSource AS;

    // player
    private playerController player;

    // can start
    private bool canStart = false;
    private bool alreadyRan = false;

    private Coroutine textAnim = null;
    private Coroutine currentAnim = null;
    private Coroutine spriteChanger = null;

    /// <summery> if set to start, play clip </summery>
    protected void Start() {
        if (startOn == cutscene.types.startType.start) StartCoroutine(waitUntilLoaded());
    }

    protected void OnEnable() {
        if (startOn == cutscene.types.startType.enable) StartCoroutine(waitUntilLoaded());
    }

    /// <summery> wait until loaded </summery>
    public IEnumerator waitUntilLoaded() {
        yield return new WaitUntil(() => GS.live.state.loaded);

        // get data
        player = playerController.mainPlayer;
        AS = transform.GetComponent<AudioSource>();
        canStart = player != null && AS != null;

        // start cutscene
        StartCoroutine(playCutscene());
    }

    /// <summery> cutscene player </summery>
    public IEnumerator playCutscene() {
        if (!canStart || alreadyRan) {
            Debug.Log($"canStart: {canStart} alreadyRan: {alreadyRan}");
            yield break;
        } alreadyRan = true;

        // start cutscene
        transform.GetChild(0).gameObject.SetActive(true);

        // set defaults
        player.CanMove = !freezePlayer;
        List<cutscene.cutsceneItem> TMPcontent = new List<cutscene.cutsceneItem>(content);
        currentAnim = null;

        imageGroup.alpha = 0f;
        textContainer.SetActive(false);

        while(backgroundGroup.alpha < 0.99f) {
            backgroundGroup.alpha = Mathf.Lerp(backgroundGroup.alpha, 1, Time.fixedDeltaTime * 4f);
            yield return 0;
        } backgroundGroup.alpha = 1f;

        while (TMPcontent.Count > 0) {
            if(spriteChanger != null) StopCoroutine(spriteChanger);
            finishedText.SetActive(false);
            currentAnim = StartCoroutine(loadCutsceneItem(TMPcontent[0]));
            
            yield return new WaitUntil(() => eevee.input.Collect("interact", "cutscene"));
            if (currentAnim != null) {
                StopCoroutine(currentAnim);
                currentAnim = StartCoroutine(loadCutsceneItem(TMPcontent[0], false));
                yield return new WaitUntil(() => eevee.input.Collect("interact", "cutscene"));
            }

            TMPcontent.RemoveAt(0);
        }

        imageGroup.alpha = 0f;
        textContainer.SetActive(false);

        // end cutscene
        while(backgroundGroup.alpha > 0.01f) {
            backgroundGroup.alpha = Mathf.Lerp(backgroundGroup.alpha, 0, Time.fixedDeltaTime * 4f);
            yield return 0;
        } backgroundGroup.alpha = 0f;
        
        StopCoroutine(spriteChanger);
        
        transform.GetChild(0).gameObject.SetActive(false);
        player.CanMove = true;
    }

    /// <summery> handle cutscene item </summery>
    private IEnumerator loadCutsceneItem(cutscene.cutsceneItem item, bool anim = true) {
        // delete old text
        if (textAnim != null) StopCoroutine(textAnim);
        textDisplay.text = "";

        // fadeSprite
        while(imageGroup.alpha > 0.01f && anim) {
            imageGroup.alpha = Mathf.Lerp(imageGroup.alpha, 0, Time.fixedDeltaTime * item.imageFadeSpeed);
            yield return 0;
        }

        spriteChanger = StartCoroutine(spriteAnimator(item.sprites, item.spriteChangeSpeed));

        // chnage text setting
        textContainer.SetActive(item.displayText);
        if (item.changeColor) colorManager.data.forceColor(item.newColor);

        song foundSong = utils.compareSong(item.songName);
        if (item.changeSong && foundSong != null) music.playSong(foundSong);
        if (item.playSound) {
            AS.clip = item.sound;
            AS.Play();
        }
        if (item.changePlayerPos) player.transform.position = item.newPos;
        
        if(anim) StartCoroutine(typeText(item.text.localise(), item.textSpeed));
        else textDisplay.text = item.text.localise();

        while(imageGroup.alpha < 0.99f && anim) {
            imageGroup.alpha = Mathf.Lerp(imageGroup.alpha, 1, Time.fixedDeltaTime * item.imageFadeSpeed);
            yield return 0;
        }
        imageGroup.alpha = 1f;

        yield return new WaitUntil(() => textAnim == null);
        finishedText.SetActive(true);
        currentAnim = null; // idk why id have to set this
    } 

    /// <summery> animate typing text </summery>
    public IEnumerator typeText(string text, float speed) {
        while (textDisplay.text.Length < text.Length) {
            textDisplay.text = text.Substring(0, textDisplay.text.Length + 1);

            yield return new WaitForSecondsRealtime(speed);
        }
    }
    
    /// <summery> animate the sprite </summery>
    public IEnumerator spriteAnimator(List<Sprite> sprites, float changeSpeed = 1f) {
        int count = 0;
        while (true) {
            imageDisplay.sprite = sprites[count];
            
            yield return new WaitForSecondsRealtime(changeSpeed);
            count++;
            if (count > sprites.Count - 1) count = 0;
        }
    }
}