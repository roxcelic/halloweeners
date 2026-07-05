using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class POMO_Animator : MonoBehaviour {
    [Header("config")]
    public List<Sprite> sprites = new List<Sprite>();
    public float frameDuration = 0.1f;

    // comp
    private SpriteRenderer sr;

    void Start() {
        sr = transform.GetComponent<SpriteRenderer>();
        StartCoroutine(anim());
    }

    // a coroutine to animate
    public IEnumerator anim() {
        while (true) {
            yield return new WaitUntil(() => sprites.Count > 0);
            int currentSprite = 0;

            while (sprites.Count > 0) {
                currentSprite = (Int32)Mathf.Clamp(currentSprite, 0, sprites.Count - 1);

                if(sprites[currentSprite] != null) sr.sprite = sprites[currentSprite];

                currentSprite++;
                if (currentSprite >= sprites.Count) currentSprite = 0;
                yield return new WaitForSecondsRealtime(frameDuration);
            }
        }
    }
}