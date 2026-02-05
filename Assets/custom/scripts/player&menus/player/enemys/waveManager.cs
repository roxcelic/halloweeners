using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

/// <summery> initialise a class to track the enemys </summery>
namespace waveManagerTypes {
    [System.Serializable]
    public class enemyTracker {
        public GameObject enemey;
        public float position;
        public float distance;

        public enemyTracker(GameObject obj, float objPosition = 0, float objDistance = 0) {
            this.enemey = obj;
            this.position = objPosition;
            this.distance = objDistance;
        }
    }
}

public class waveManager : MonoBehaviour {
    [Header("spawn conditions")]
    [Range(0, 25f)] public float spawnRadius;
    [Range(0, 5f)] public float groundCheckDistance;
    [Range(0, 5f)] public float trackingUpdate;
    [Range(0f, 2.5f)] public float spawnRate = 1.5f;
    [Range(1f, 15f)] public float waveDelay = 15f;
    [Range(1f, 15f)] public float spawnDelay = 1f;

    public int spawnAmount = 5;

    public LayerMask groundLayer;

    [Header("components")]
    public List<GameObject> enemys;
    private playerController player;
    public TMP_Text T_display;

    [Header("data")]
    private List<GameObject> currentEnemys = new List<GameObject>();
    private List<GameObject> spawnedEnemys = new List<GameObject>();
    [Min(1)]public int wave;

    [Header("display")]
    public float timeUntilNextWave;
    public bool spawning = false;

    [Header("override")]
    public bool generateWaves = true;
    public bool useDifficultyLength = true;

    [Header("text")]
    public sys.Text spawningMessage = new sys.Text();

    [Header("config")]
    public int len = 5;
    public int baseWaves = 3;

    /// <summery> this is called when the waves begin spawning </summery>
    ///  -- this is called in LoadingScreen
    public void Begin() {
        if (generateWaves) StartCoroutine(startWaves());
        else StartCoroutine(trackEnemyCount());
    }

    /// <summery> the main function to start the waves </summery>
     public IEnumerator startWaves() {
        // track the enemys
        spawning = true;
        Coroutine enemyTracker = StartCoroutine(trackEnemies());

        // wait until the name wave begins
        timeUntilNextWave = waveDelay;
        while (timeUntilNextWave > 0) {
            timeUntilNextWave -= Time.deltaTime;
            T_display.text = $"|{timeUntilNextWave}|";

            yield return 0;
        }
                
        // display the message for the enemys spawning
        T_display.text = $"|{spawningMessage.localise()}|";
        for (int i = 0; i < (spawnAmount * Mathf.Round(spawnRate * wave)); i++) {
            // chose a random enemy from the list, i will have the change where this is held at some point
            GameObject chosenEnemy = enemys[UnityEngine.Random.Range(0, enemys.Count)];
            Vector3 chosenLocation = new Vector3();

            // spawn an enemy by attempting to find a position and if its invalid wait a second before attempting again
            while (!(chosenLocation = transform.localPosition + new Vector3(UnityEngine.Random.Range(-spawnRadius, spawnRadius), 0, UnityEngine.Random.Range(-spawnRadius, spawnRadius))).checkPosition(3, groundCheckDistance)) {
                Debug.Log($"position is invalid {chosenLocation}");
                yield return new WaitForSeconds(1f);
            }

            // spawn the enemy and add it to the lists
            GameObject enemy = Instantiate(chosenEnemy, chosenLocation, Quaternion.identity);
            currentEnemys.Add(enemy);
            spawnedEnemys.Add(enemy);

            // wait for next
            yield return new WaitForSeconds(spawnDelay);
        }

        // spawning over
        spawning = false;

        // wait until the next wave then inciment
        yield return new WaitUntil(() => currentEnemys.Count == 0 && !spawning);
        wave++;
        
        // destroy all currently spawned enemys (this is for if they are stuck on their death animation on the wave transition)
        foreach (GameObject obj in spawnedEnemys) Destroy(obj);
        spawnedEnemys = new List<GameObject>();

        StopCoroutine(enemyTracker); // stop the enemys from being tracked and  creating a memory leak
        if(wave <= PlayerPrefs.GetInt("difficulty", 1) + baseWaves || !useDifficultyLength) Begin(); // start the next wave
        else {
            T_display.text = $"|0/{spawnAmount * Mathf.Round(spawnRate * wave)}|";
            Debug.Log("you win lwk");
        }
    }

    /// <summery>  track the amount of enemys spawned </summery>
    public IEnumerator trackEnemies() {
        while (true) {

            // update the currently alive enemys list
            if (currentEnemys.Count != 0) {
                List<GameObject> newlist = new List<GameObject>();

                foreach (GameObject enm in currentEnemys) {
                    if (enm != null && !enm.transform.GetComponent<EN_base>().dead) {
                        newlist.Add(enm);
                    }
                }

                currentEnemys = newlist;
            }

            // wait for the tracking update
            yield return new WaitForSeconds(trackingUpdate);

            // display the results
            if (!spawning) T_display.text = $"|{currentEnemys.Count}/{spawnAmount * Mathf.Round(spawnRate * wave)}|";
        }
    }

    /// <summery> track enemys without spawning waves </summery>
    public IEnumerator trackEnemyCount() {
        while (true) {
            // find all enemys
            GameObject[] Tenemys = GameObject.FindGameObjectsWithTag("Enemy");
            List<EN_base> enemys = new List<EN_base>();

            // for each enemy found, check if its an enemy, if it is add it to the list
            foreach (GameObject enemy in Tenemys) {
                EN_base TEMPenemy = enemy.transform.GetComponent<EN_base>();
                if (TEMPenemy != null) enemys.Add(TEMPenemy);
            }

            // count the amount of alive enemys and start a coroutine to destroy the enemy if it isnt
            int aliveEnemys = 0;
            foreach (EN_base enemy in enemys) {
                if (!enemy.dead) aliveEnemys++;
                else StartCoroutine(killAfter(enemy.transform.gameObject)); // this will make a bunch of coroutines for one enemy, this needs fixing
            }
            
            // display the text result
            T_display.text = $"{(aliveEnemys > 0 ? $"|-{String.Format("{0:00000}", aliveEnemys)}-|" : $"|-{genString()}-|")}";

            // loop
            yield return new WaitForSeconds(trackingUpdate);
        }
    }

    /// <summery> destroys the enemy after a given period of time </summery>
    public IEnumerator killAfter(GameObject target, float duration = 5f) {
        yield return new WaitForSeconds(duration);
        Destroy(target);
    }

    /// <summery> a util to make a random string </summery>
    private string genString() {
        System.Random random = new System.Random();

        return new string(Enumerable.Repeat(sys.var.keywords.characters, len)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}