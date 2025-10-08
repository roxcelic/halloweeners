using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace waveManagerTypes {

    [System.Serializable]
    public class enemyTracker {
        public GameObject enemey;
        public float position;

        public enemyTracker(GameObject obj, float objPosition = 0) {
            this.enemey = obj;
            position = objPosition;
        }
    }
}

public class waveManager : MonoBehaviour {
    [Header("components")]
    public List<GameObject> enemys;
    public movement player;

    [Header("spawn conditions")]
    [Range(0, 25f)] public float spawnRadius;
    [Range(0, 5f)] public float groundCheckDistance;
    [Range(0, 5f)] public float trackingUpdate;
    
    [Range(0f, 2.5f)] public float spawnRate = 1.5f;
    public int spawnAmount = 5;

    [Range(1f, 15f)] public float waveDelay = 15f;
    [Range(1f, 15f)] public float spawnDelay = 1f;

    public string groundLayer;

    [Header("data")]
    public List<waveManagerTypes.enemyTracker> currentEnemys;
    public List<GameObject> spawnedEnemys;
    [Min(1)]public int wave;

    [Header("tracker display")]
    public TMP_Text T_display;
    public string T_nullCharacter;
    public string T_activeCharacter;
    public string T_centerCharacter;
    private int size = 10;

    [Header("display")]
    public float timeUntilNextWave;
    public bool spawning = false;

    public void Begin() {
        StartCoroutine(startWaves());
    }

    public bool checkPosition(Vector3 position) {
        RaycastHit hit;
        Vector3 targetDirection = Vector3.down;

        if (Physics.Raycast(position, transform.TransformDirection(targetDirection), out hit, groundCheckDistance)) {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer(groundLayer)) {
                return false; 
            }

            Debug.DrawRay(position, transform.TransformDirection(targetDirection) * hit.distance, Color.red, 4); 
        
            return true;
        }

        return false;
    }

    public float compareLocation(GameObject Target) {
        float value = AngleDifference(transform.eulerAngles.y, Quaternion.LookRotation(Target.transform.position - transform.position).eulerAngles.y); // important
        
        if (value > 180) value -= 360;
        value *= 20;
        value /= 360;

        return (float)Math.Round(value);
    }

    /*
        stackoverflow.com/questions/28036652/finding-the-shortest-distance-between-two-angles
    */
    public float AngleDifference( float angle1, float angle2 ) {
        float diff = ( angle2 - angle1 + 180 ) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }


    public IEnumerator startWaves() {
        // track the enemys
        spawning = true;
        StartCoroutine(trackEnemies());

        timeUntilNextWave = waveDelay;
        Debug.Log($"set time until next wave to: {timeUntilNextWave}");
        while (timeUntilNextWave > 0) {
            Debug.Log($"wave timer ({timeUntilNextWave}) updated via: {Time.fixedDeltaTime} to: {timeUntilNextWave -= Time.fixedDeltaTime}");
            T_display.text = $"next wave in: {timeUntilNextWave}";
            yield return 0;
        }
                
        
        for (int i = 0; i < (spawnAmount * Mathf.Round(spawnRate * wave)); i++) {
            StartCoroutine(attemptSpawn());
            yield return new WaitForSeconds(spawnDelay);
        }

        spawning = false;
        Debug.Log("finished spawning");

        yield return new WaitUntil(() => currentEnemys.Count == 0 && !spawning);

        Debug.Log("all enemys are dead");
        
        wave++;
        
        foreach (GameObject obj in spawnedEnemys) Destroy(obj);
        spawnedEnemys = new List<GameObject>();

        Begin(); // start the next wave
    }

    public IEnumerator attemptSpawn() {
        GameObject chosenEnemy = enemys[UnityEngine.Random.Range(0, enemys.Count)];
        Vector3 chosenLocation = new Vector3();

        while (!checkPosition(chosenLocation = transform.localPosition + new Vector3(UnityEngine.Random.Range(0, spawnRadius), 0, UnityEngine.Random.Range(0, spawnRadius)))) {
            Debug.Log($"position: {chosenLocation} failed the check trying again...");
            yield return new WaitForSeconds(1f);
        }

        GameObject enemy = Instantiate(chosenEnemy, chosenLocation, Quaternion.identity);
        currentEnemys.Add(new waveManagerTypes.enemyTracker(enemy));
        spawnedEnemys.Add(enemy);
    }

    public IEnumerator trackEnemies() {
        while (true) {
            // get positions
            if (currentEnemys.Count == 0) yield return new WaitForSeconds(trackingUpdate);
            else {
                List<waveManagerTypes.enemyTracker> newlist = new List<waveManagerTypes.enemyTracker>();

                foreach (waveManagerTypes.enemyTracker enm in currentEnemys) {
                    if (enm.enemey != null && !enm.enemey.transform.GetComponent<EN_base>().dead) {
                        newlist.Add(new waveManagerTypes.enemyTracker(enm.enemey, compareLocation(enm.enemey)));
                    }
                }
                currentEnemys = newlist;

                // tracker
                // organise trackers
                Dictionary<float, string> enemys = new Dictionary<float, string>();
                foreach (waveManagerTypes.enemyTracker enm in currentEnemys) {
                    if (enemys.ContainsKey(enm.position)) enemys[enm.position] = $"{enemys[enm.position]}{T_activeCharacter}";
                    else enemys.Add(enm.position, T_activeCharacter);
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

}
