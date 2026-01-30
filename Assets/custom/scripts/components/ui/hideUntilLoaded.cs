using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

public class hideUntilLoaded : MonoBehaviour {
    public enum loadType {
        initial,
        player
    }
    public enum hideType {
        canvasGroup,
        deactivateChild
    }

    public hideType type;
    public loadType loadLevel;

    private CanvasGroup cg;

    void Start() {
        cg = transform.GetComponent<CanvasGroup>();
        StartCoroutine(wait());
    }

    public IEnumerator wait() {
        switch (type) {
            case hideType.canvasGroup:
                cg.alpha = 0;
                yield return new WaitUntil(() => getLoaded());
                cg.alpha = 1;

                break;
            case hideType.deactivateChild: default:
                transform.GetChild(0).gameObject.SetActive(false);
                yield return new WaitUntil(() => getLoaded());
                transform.GetChild(0).gameObject.SetActive(true);

                break;
        }
    }

    public bool getLoaded() {
        bool value = false;
        switch (loadLevel) {
            case loadType.player:
                if (playerController.mainPlayer == null) return false;
                value =  playerController.mainPlayer.loaded;

                break;
            case loadType.initial: default:
                value =  GS.live.state.loaded;

                break;
        }

        return value;
    }

}
