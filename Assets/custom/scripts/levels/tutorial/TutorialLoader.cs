using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace tutorialTypes {
    [System.Serializable]
    public class tutorialItem {
        public string name; // no reason other than to be clear
        public GameObject item;
        public float riseAmount;
        public sys.Text text = new sys.Text();
        public string optionalWaitForInput = "";
        public bool optionalWaitForEnemyDeath = false;
        public bool disableAtEnd = true;
    }
}

public class TutorialLoader : MonoBehaviour {
    public static TutorialLoader instance;

    [Header("data")]
    public List<tutorialTypes.tutorialItem> objects;
    public bool A_continue = false;
    [Range(0f, 5f)] public float speed = 2.2f;
    public GameObject finalCanvas;

    /// <summery> start the program </summery>
    void Start() {
        instance = this;
    
        StartCoroutine(workIt());
    }

    /// <summery> work its way through the loaded objects </summery>
    public IEnumerator workIt() {
        int index = 0;
        yield return new WaitUntil(() => playerController.mainPlayer != null);
        yield return new WaitUntil(() => playerController.mainPlayer.loaded);

        while(index < objects.Count) {
            // display text
            if (objects[index].text != new sys.Text()) sys.utils.displayOnPlayer(objects[index].text);
            // turn it on and push it up
            objects[index].item.SetActive(true);
            Vector3 startPos = objects[index].item.transform.position;
            Vector3 endPos = startPos + new Vector3(0, objects[index].riseAmount);
            while (Vector3.Distance(objects[index].item.transform.position, endPos) > 0.01f && objects[index].riseAmount != 0) {
                objects[index].item.transform.position = Vector3.Lerp(objects[index].item.transform.position, endPos, Time.deltaTime * speed);
                yield return 0;
            }
            objects[index].item.transform.position = endPos;

            // wait until told
            Debug.Log("starting wait");
            yield return new WaitUntil(() => A_continue 
                || (objects[index].optionalWaitForInput != "" 
                && eevee.input.Check(objects[index].optionalWaitForInput))
                || (objects[index].optionalWaitForEnemyDeath 
                && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            );
            Debug.Log("finished wait");
            
            A_continue = false; // reset

            // lower and loop
            while (Vector3.Distance(objects[index].item.transform.position, startPos) > 0.01f && objects[index].riseAmount != 0) {
                objects[index].item.transform.position = Vector3.Lerp(objects[index].item.transform.position, startPos, Time.deltaTime * speed);
                yield return 0;
            }
            if(objects[index].disableAtEnd) objects[index].item.SetActive(false);
            index++;
        }
        Debug.Log("finished");

        finalCanvas.SetActive(true);
        GS.live.state.menued = true;
        Cursor.lockState =  CursorLockMode.None;
        Cursor.visible = true;
        playerController.mainPlayer.CanMove = false;
    }
}