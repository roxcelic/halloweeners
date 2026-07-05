using UnityEngine;

public class AU_kunai : MonoBehaviour {
    [Header("kunais")]
    /// <summery> yes i could not hard code this but i dont care loser </summery>
    public Transform kunai1;
    public Transform kunai2;
    public Transform kunai3;
    public Transform kunai4;

    [Header("conf")]
    public GameObject prefab;

    public GameObject spawnKunai(ref Transform kunai) {
        GameObject spawnedKunai = Instantiate(prefab, new Vector3(), Quaternion.identity);
        spawnedKunai.transform.position = kunai.position;
        spawnedKunai.transform.rotation = kunai.rotation;
        return spawnedKunai;
    }

    /// <summery> animation functions </summery>
    public void spawnKunai1() {spawnKunai(ref kunai1);}
    public void spawnKunai2() {spawnKunai(ref kunai2);}
    public void spawnKunai3() {spawnKunai(ref kunai3);}
    public void spawnKunai4() {spawnKunai(ref kunai4);}
}