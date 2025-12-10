using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class colorRadiency : MonoBehaviour {
    public List<Color> colors;
    [Range(0f, 1f)] public float delay = 0.3f;
    private int ci = 0;
    public bool active = true;

    void Start(){StartCoroutine(main());}

    public IEnumerator main() {
        while (true) {
            colorManager.data.forceColor(colors[ci]);

            ci++;
            if (ci >= colors.Count) ci = 0;
            yield return new WaitForSecondsRealtime(delay);
            yield return new WaitUntil(() => active);
        }
    }

}