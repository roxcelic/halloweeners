using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/*
    To use this i will need to have a `levelType` and 'difficulty' saved in player prefs
    
    > PlayerPrefs.SetString("levelType", 0);
    > PlayerPrefs.GetString("levelType", 0);
    > PlayerPrefs.SetInt("difficulty", 0);
    > PlayerPrefs.GetInt("difficulty", 0);
*/
public class RL_Loader : MonoBehaviour {
    [Serializable]
    public class levelToLoad {
        public string name;
        public GameObject refrence;
        public bool defaultLevel;

        public levelToLoad() {}
    }

    // the levels
    public List<levelToLoad> levels;

    /// <summery> start the loading co-routine </summery>    
    void Start() {StartCoroutine(load());}

    /// <summery> begins the loading </summery>
    public IEnumerator load() {
        yield return new WaitUntil(() => sys.var.components.loadingScreen() != null);
        sys.var.components.loadingScreen().spawnCondition("RL_Loader_1", false); // add the spawn condition

        string levelType = PlayerPrefs.GetString("levelType", "basic");
        int difficulty = PlayerPrefs.GetInt("difficulty", 0);

        levelToLoad selectedLevel = null;
        foreach(levelToLoad item in levels) if (item.name == levelType) selectedLevel = item;
        if (selectedLevel == null) foreach(levelToLoad item in levels) if (item.defaultLevel) selectedLevel = item;


        // set it as active
        selectedLevel.refrence.SetActive(true);

        // set the difficulty

        sys.var.components.loadingScreen().spawnCondition("RL_Loader_1", true); // close the spawn condition        
    }
}
