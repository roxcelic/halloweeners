using UnityEngine;

public class RL_ContinueOnEnemyDeath : MonoBehaviour {
    [Header("config")]
    public bool waitForEnemysToSpawn = true;
    
    [Header("data")]
    public int enemysLeft = 0;
    private int maxEnemyCount = 0;

    void Update() {
        enemysLeft = 0;        
        foreach(GameObject enm in GameObject.FindGameObjectsWithTag("Enemy")) if (!enm.transform.GetComponent<EN_base>().dead) enemysLeft++;
        maxEnemyCount = Mathf.Max(maxEnemyCount, enemysLeft);
    
        if (enemysLeft == 0 && (!waitForEnemysToSpawn || maxEnemyCount > 0)) roomLoader.instance.Next();
    }
}