using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*
    To use this library i will need to have a `saveSlot` saved in player prefs
    
    > PlayerPrefs.SetInt("saveSlot", 0);
    > PlayerPrefs.GetInt("saveSlot", 0);
*/
namespace save {

    /*
        The save data class, this will be updated as my needs increase
    */
    [System.Serializable]
    public class saveData {
        [Header("config")]
        public bool dev = false;
        public bool instantRespawn = false;
        public string language = "English";

        public string ver = "dev-0.1.0-11";

        public bool firstTimeInPauseMenu = false;

        [Header("inventory")]
        public string currentAttack = "";
        public string currentAbility = "";
        public attack.attackData currentAttackData = new attack.attackData();
        public List<AVdata.savedAttack> savedAttacks = new List<AVdata.savedAttack>();

        public saveData() {
            this.ver = var.ver;
        }
    }

    /*
        a simple wrapperclass
    */
    [System.Serializable]
    public class fullSave {
        public List<saveData> saves = new List<saveData>();
        
        public fullSave () {this.saves = new List<saveData>(){new saveData()};}
    }

    /*
        a class to store the data
    */
    public class var {
        public static string ver = "dev-0.1.0-11";
        public static string ConfPath = Path.Combine(Application.persistentDataPath, "saveData.json");

        public static fullSave saves = new fullSave();
    }

    /* 
        a class for the user to interact with
    */
    public class getData {
        static getData() {
            var.saves = data.getSaves();
        }

        // a function to check if youre a dev
        public static bool isDev() {
            return viewSave().dev;
        }

        // a function to view a save
        public static saveData viewSave(saveData fallback = null) {
            if (fallback == null) fallback = new saveData();

            if (var.saves.saves.Count == 0) {
                var.saves.saves.Add(fallback);
                PlayerPrefs.SetInt("saveSlot", 0);
            }
            
            int currentSave = clampSave(PlayerPrefs.GetInt("saveSlot", 0));         

            return var.saves.saves[currentSave];
        }

        // a function to edit the save
        public static void save(saveData newSave) {
            int currentSave = clampSave(PlayerPrefs.GetInt("saveSlot", 0));

            if (var.saves.saves.Count == 0) {
                var.saves.saves.Add(newSave);
                PlayerPrefs.SetInt("saveSlot", 0);
            } else {
                var.saves.saves[currentSave] = newSave;
            }

            data.push(); // save the new data
        }

        // a util function to get the clamp the save to the max
        private static int clampSave(int currentSave) {
            currentSave = Math.Clamp(currentSave, 0, var.saves.saves.Count - 1); 

            return currentSave;  
        }
    }

    /*
        a class to store and recall the save data
    */
    public class data {
        // a function to save the current data
        public static void push() {
            File.WriteAllText(var.ConfPath, JsonUtility.ToJson(var.saves));
        }

        public static fullSave getSaves() {
            if (File.Exists(var.ConfPath)) {
                string json = File.ReadAllText(var.ConfPath);

                return JsonUtility.FromJson<fullSave>(json);
            }

            return new fullSave();
        }
    }
}