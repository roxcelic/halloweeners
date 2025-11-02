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
    public playerController player;

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
    public List<GameObject> currentEnemys;
    public List<GameObject> spawnedEnemys;
    [Min(1)]public int wave;

    [Header("tracker display")]
    public TMP_Text T_display;

    [Header("display")]
    public float timeUntilNextWave;
    public bool spawning = false;

    [Header("override")]
    public bool generateWaves = true;

    public void Begin() {
        if (generateWaves) StartCoroutine(startWaves());
        else StartCoroutine(trackEnemyCount());
    }

    public bool checkPosition(Vector3 position) {
        RaycastHit hit;
        Vector3 targetDirection = Vector3.down;

        if (Physics.Raycast(position, transform.TransformDirection(targetDirection), out hit, groundCheckDistance)) {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer(groundLayer)) {
                return false; 
            }

            return true;
        }

        return false;
    }


    public IEnumerator startWaves() {
        // track the enemys
        spawning = true;
        StartCoroutine(trackEnemies());

        timeUntilNextWave = waveDelay;
        while (timeUntilNextWave > 0) {
            timeUntilNextWave -= Time.deltaTime;
            T_display.text = $"{timeUntilNextWave}";

            yield return 0;
        }
                
        T_display.text = $"spawning...";
        for (int i = 0; i < (spawnAmount * Mathf.Round(spawnRate * wave)); i++) {
            /*
                This was a function but its better to just put it here
            */
            GameObject chosenEnemy = enemys[UnityEngine.Random.Range(0, enemys.Count)];
            Vector3 chosenLocation = new Vector3();

            while (!checkPosition(chosenLocation = transform.localPosition + new Vector3(UnityEngine.Random.Range(-spawnRadius, spawnRadius), 0, UnityEngine.Random.Range(-spawnRadius, spawnRadius)))) {
                Debug.Log($"position: {chosenLocation} failed the check trying again...");
                yield return new WaitForSeconds(1f);
            }

            GameObject enemy = Instantiate(chosenEnemy, chosenLocation, Quaternion.identity);
            currentEnemys.Add(enemy);
            spawnedEnemys.Add(enemy);

            // wait for next
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

    /*
        Basic track
    */
    public IEnumerator trackEnemies() {
        while (true) {
            // get positions
            if (currentEnemys.Count != 0) {
                List<GameObject> newlist = new List<GameObject>();

                foreach (GameObject enm in currentEnemys) {
                    if (enm != null && !enm.transform.GetComponent<EN_base>().dead) {
                        newlist.Add(enm);
                    }
                }

                currentEnemys = newlist;
            }

            yield return new WaitForSeconds(trackingUpdate);

            if (!spawning) T_display.text = $"{currentEnemys.Count}/{spawnAmount * Mathf.Round(spawnRate * wave)}";
        }
    }

    /*
        Just enemy count
    */
    public IEnumerator trackEnemyCount() {
        while (true) {
            GameObject[] Tenemys = GameObject.FindGameObjectsWithTag("Enemy");
            List<EN_base> enemys = new List<EN_base>();

            foreach (GameObject enemy in Tenemys) {
                EN_base TEMPenemy = enemy.transform.GetComponent<EN_base>();
                if (TEMPenemy != null) enemys.Add(TEMPenemy);
            }

            int aliveEnemys = 0;
            foreach (EN_base enemy in enemys) if (!enemy.dead) aliveEnemys++;

            T_display.text = $"{aliveEnemys}/{enemys.Count}";

            yield return new WaitForSeconds(trackingUpdate);
        }
    }
}
