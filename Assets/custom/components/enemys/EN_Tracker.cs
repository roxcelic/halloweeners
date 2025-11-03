using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

// arhrhrhrhrhhr
public class EN_Tracker : MonoBehaviour {

    [Header("tracker display")]
    public TMP_Text T_display;
    [Range(0, 5f)] public float trackingUpdate;
    public string T_nullCharacter;
    public string T_activeCharacter;
    public string T_centerCharacter;
    private int size = 10;
    public List<waveManagerTypes.enemyTracker> currentEnemys;

    void Start() {
        StartCoroutine(trackEnemies());
    }

    void Update() {
        List<waveManagerTypes.enemyTracker> TMPe = new List<waveManagerTypes.enemyTracker>();
        
        foreach(GameObject enm in GameObject.FindGameObjectsWithTag("Enemy")) {
            TMPe.Add(new waveManagerTypes.enemyTracker(enm));
        } 

        currentEnemys = TMPe;
    }

    #region  utils
    public float compareLocation(GameObject Target) {
        float value = AngleDifference(transform.eulerAngles.y, Quaternion.LookRotation(Target.transform.position - transform.position).eulerAngles.y); // important
        
        if (value > 180) value -= 360;
        value *= 20;
        value /= 360;

        return (float)Math.Round(value);
    }

    public string clacColor(float distance) {
        distance /= 2;
        List<string> hexValues = new List<string>(){"f","e","d","c","b","a","9","8","7","6","5","4","3","2","1"};
        int clampedDistance = Math.Clamp((int)distance, 0, hexValues.Count - 1);

        return $"{hexValues[clampedDistance]}{hexValues[clampedDistance]}";
    }

    /*
        stackoverflow.com/questions/28036652/finding-the-shortest-distance-between-two-angles
    */
    public float AngleDifference( float angle1, float angle2 ) {
        float diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }
    #endregion

    #region tracking
    public IEnumerator trackEnemies() {
        while (true) {
            // get positions
            if (currentEnemys.Count == 0) yield return new WaitForSeconds(trackingUpdate);
            else {
                List<waveManagerTypes.enemyTracker> newlist = new List<waveManagerTypes.enemyTracker>();

                foreach (waveManagerTypes.enemyTracker enm in currentEnemys) {
                    if (enm.enemey != null && !enm.enemey.transform.GetComponent<EN_base>().dead) {
                        newlist.Add(new waveManagerTypes.enemyTracker(enm.enemey, compareLocation(enm.enemey), Vector3.Distance(transform.position, enm.enemey.transform.position)));
                    }
                }
                currentEnemys = newlist;

                // tracker
                // organise trackers
                Dictionary<float, string> enemys = new Dictionary<float, string>();
                foreach (waveManagerTypes.enemyTracker enm in currentEnemys) {
                    if (enemys.ContainsKey(enm.position)) enemys[enm.position] = $"<color=#ff0000{clacColor(enm.distance)}>{enemys[enm.position]}{T_activeCharacter}</color>";
                    else enemys.Add(enm.position, $"<color=#ff0000{clacColor(enm.distance)}>{T_activeCharacter}</color>");
                }

                // left
                string left = "";
                for (int i = 0; i < size; i++) {
                    float key = -(10 - i);
                    string trackingData = enemys.ContainsKey(key) ? enemys[key] : "";

                    left += $"{trackingData}{T_nullCharacter}";
                }

                // right
                string right = "";
                for (int i = 0; i < size; i++) {
                    string trackingData = enemys.ContainsKey(i) ? enemys[i] : "";

                    right += $"{T_nullCharacter}{trackingData}";
                }

                T_display.text = $"{left}{T_centerCharacter}{right}";

                yield return new WaitForSeconds(trackingUpdate);
            }
        }
    }
    #endregion

}
