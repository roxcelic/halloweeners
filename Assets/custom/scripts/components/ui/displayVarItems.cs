using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class displayVarItems : MonoBehaviour {
    public enum animationType {
        scale,
        position
    }

    [Header("amount")]
    [Range(0, 10)] public int horizontal = 5;
    [Range(0, 10)] public int verticle = 5;

    public int width = 100;
    public int height = 100;

    [Header("comp")]
    public GameObject toSpawn;

    [Header("config")]
    public bool matchSize = true;
    public Vector2 offset = new Vector2();

    [Header("animation")]
    public animationType animType;

        [Header("animation scale")]
        public Vector2 startScale = new Vector2(0.5f, 0.2f);
        public Vector2 endScale = new Vector2(0.5f, 0.5f);
    
    [Header("data")]
    public bool spawning = false;

    /// <summery> spawn all children </summery>
    protected virtual void OnEnable() {
        if (matchSize) {
            RectTransform selfRect = transform.GetComponent<RectTransform>();
            
            width = (int)selfRect.sizeDelta.x;
            height = (int)selfRect.sizeDelta.y;
        }

        spawning = false;

        StartCoroutine(spawnChildren());
    }

    /// <summery> a function to reset </summery>
    public void reset() {
        OnDisable();
        OnEnable();
    }

    /// <summery> destroy all kids </summery>
    protected virtual void OnDisable() {
        foreach(Transform child in transform) Destroy(child.gameObject);
    }

    /// <summery> an overrideable void for custom components </summery>
    public virtual GameObject editSpawn(GameObject spawned, Vector2 set) {
        return spawned;
    }

    /// <summery> animate spawn </summery>
    public IEnumerator spawnChildren() {
        spawning = true;

        Vector2 currentPos = new Vector2();

        int tmpX = 0;
        int tmpY = 0;

        while (verticle > tmpY && transform.gameObject.activeSelf) {
            while(horizontal > tmpX) {
                currentPos = new Vector2(
                    ((width / horizontal) * tmpX) - (width / 2),
                    (height - ((height / verticle) * tmpY)) - (height / 2)
                ) + offset;

                RectTransform trans = editSpawn(Instantiate(toSpawn, new Vector3(), Quaternion.identity), new Vector2(tmpX, tmpY)).transform.GetComponent<RectTransform>();
                trans.SetParent(transform);

                tmpX++;
                
                // an animation switch
                switch(animType) {
                    case animationType.scale:
                        trans.localPosition = currentPos;
                        trans.localScale = startScale;
                        StartCoroutine(transitionA(trans, endScale));

                        break;
                    case animationType.position:
                        trans.localPosition = new Vector2();
                        StartCoroutine(transitionB(trans, currentPos));

                        break;
                }
                yield return new WaitForSecondsRealtime(0.05f);
            }

            tmpX = 0;
            tmpY++;
        }

        spawning = false;
    }

    /// <summery> a transition animation, scale </summery<
    public IEnumerator transitionA(Transform target, Vector2 targetScale) {
        while (target != null && target.gameObject.activeSelf && Vector2.Distance(target.localScale, targetScale) > 0.01f) {
            target.localScale = Vector2.Lerp(target.localScale, targetScale, Time.fixedDeltaTime * 5f);

            yield return 0;
        }
        if (target != null) target.localScale = targetScale;
    }

    /// <summery> a transition animation, position </summery>
    public IEnumerator transitionB(Transform target, Vector2 targetPosition) {
        while (target != null && target.gameObject.activeSelf && Vector2.Distance(target.localPosition, targetPosition) > 0.01f) {
            target.localPosition = Vector2.Lerp(target.localPosition, targetPosition, Time.fixedDeltaTime * 5f);

            yield return 0;
        }
        if (target != null) target.localPosition = targetPosition;
    }
}