using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class displayPlayerText : MonoBehaviour {
    [Header("config")]
    public string playerTag = "Player";
    public List<sys.Text> textToDisplay = new List<sys.Text>();
    
    
    public bool exit = false;
    public bool destroy = true;

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == playerTag && !exit) {
            other.transform.GetComponent<textDisplay>().addText(textToDisplay); 
            if (destroy) Destroy(transform.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == playerTag && exit) {
            other.transform.GetComponent<textDisplay>().addText(textToDisplay); 
            if (destroy) Destroy(transform.gameObject);
        }
    }
}