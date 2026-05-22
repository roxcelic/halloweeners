using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class displayPlayerText : MonoBehaviour {
    [Header("config")]
    public string playerTag = "Player";
    public sys.Text textToDisplay = new sys.Text();
    
    public bool exit = false;
    public bool displayKey = false;
    public bool destroy = true;

    public int maxRuns = 0;
    public int runs = 0;

    void OnTriggerEnter(Collider other){
        if (runs >= maxRuns && maxRuns != 0) return;
        if ((other.gameObject.tag == sys.var.config.playerTag || other.gameObject.tag == sys.var.config.otherPlayerTag) && !exit) {
            
            if (displayKey) sys.utils.displayOnPlayer(new sys.Text(sys.text.displayKeyButton(textToDisplay.localise())));
            else sys.utils.displayOnPlayer(textToDisplay);
            
            runs++;
            if (destroy) Destroy(transform.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if (runs >= maxRuns && maxRuns != 0) return;
        if ((other.gameObject.tag == sys.var.config.playerTag || other.gameObject.tag == sys.var.config.otherPlayerTag) && exit) {
            
            if (displayKey) sys.utils.displayOnPlayer(new sys.Text(sys.text.displayKeyButton(textToDisplay.localise())));
            else sys.utils.displayOnPlayer(textToDisplay);
            
            runs++;
            if (destroy) Destroy(transform.gameObject);
        }
    }
}