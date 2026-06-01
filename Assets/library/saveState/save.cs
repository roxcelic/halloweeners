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
        public string name = "";
        public bool dev = false;
        public bool lockSpeedDisplay = true;

        public bool firstTimeInPauseMenu = false;

        [Header("config")]
        public string mainColor = "#c0000D";
        public int speedAnimation = 0;
        public bool displayPlayerInfo = true;
        public bool hideUi = false; 

        [Header("inventory")]
        public string currentAttack = "";
        public string currentAbility = "";
        public attack.attackData currentAttackData = new attack.attackData();
        public List<AVdata.savedAttack> savedAttacks = new List<AVdata.savedAttack>();
        public List<string> unlockedStory = new List<string>();

        [Header("levels")]
        public List<string> unlockedLevels = new List<string>();

        public saveData() {}
    }

    [System.Serializable]
    public class fullConfig {
        // basic settings stuff
        public string ver = "dev-0.1.0-13";
        public string language = "English";
        public bool instantRespawn = false;
        
        // game
        public float sense = 1;
        public float fov = 60;

        // volume
        public float volume_master = 0f;
        public float volume_music = 0f;
        public float volume_sfx = 0f;

        // gama
        public float gama = 0f;

        public fullConfig() {}
    }

    /*
        a simple wrapperclass
    */
    [System.Serializable]
    public class fullSave {
        public List<saveData> saves = new List<saveData>();
        public fullConfig config = new fullConfig();
        
        public fullSave () {this.saves = new List<saveData>(){new saveData()};}
    }

    /*
        a class to store the data
    */
    public class var {
        public static string ver = "dev-0.1.0-13";
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

        // a function to get the full config
        public static fullConfig config() {
            return var.saves.config;
        }

        // a function to save the config
        public static void saveConfig(fullConfig newConf) {
            var.saves.config = newConf;
            data.push();
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

            // some verifaction
            //  -- specifically for weapons
            foreach(AVdata.savedAttack attack in newSave.savedAttacks) if(attack.attackName == "") newSave.savedAttacks.Remove(attack);

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

        // a function to make up to an amount of saves
        public static void makeSaves(int saveCap = 5) {
            fullSave allSaves = getSaves();
            while(allSaves.saves.Count < saveCap) {
                var.saves.saves.Add(new saveData());
                data.push();
                allSaves = getSaves();
            }
        }
    }

    /*
        a class to store sone siple utils
    */
    public static class utils {
        // a function to get the current color
        public static Color getColor() {
            if (ColorUtility.TryParseHtmlString( getData.viewSave().mainColor, out Color myColor)) {
                return myColor;
            } else {
                return Color.red;
            }
        }

        public static string getHexColor() {
            if (ColorUtility.TryParseHtmlString( getData.viewSave().mainColor, out Color myColor) && getData.viewSave().mainColor.Length == 7) {
                return getData.viewSave().mainColor;
            } else {
                return "#c0000D";
            }
        }

        // a function to get the dev status
        public static bool getDev() {
            return getData.viewSave().dev;
        }
    }
}