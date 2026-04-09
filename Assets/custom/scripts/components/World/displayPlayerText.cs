using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class displayPlayerText : MonoBehaviour {
    [Header("config")]
    public string playerTag = "Player";
    public sys.Text textToDisplay = new sys.Text();
    
    public bool exit = false;
    public bool destroy = true;

    void OnTriggerEnter(Collider other){
        Debug.Log($"tag: {other.gameObject.tag}");
        if ((other.gameObject.tag == sys.var.config.playerTag || other.gameObject.tag == sys.var.config.otherPlayerTag) && !exit) {
            sys.utils.displayOnPlayer(textToDisplay);
            if (destroy) Destroy(transform.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if ((other.gameObject.tag == sys.var.config.playerTag || other.gameObject.tag == sys.var.config.otherPlayerTag) && exit) {
            sys.utils.displayOnPlayer(textToDisplay);
            if (destroy) Destroy(transform.gameObject);
        }
    }
}