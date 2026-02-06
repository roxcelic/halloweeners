using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using sys;

/**
* The animtions required for the base
*   Idle
*   Attack[n ? 0] -- (if you have 1 attack just 'attack' but all after start at 'attack[1]')
*/


[CreateAssetMenu(fileName = "new attack", menuName = "attacks/3D")]
public class AT_3D : AT_base {
    [Header("3d config")]
    public GameObject weapon;
    [Min(1)] public int animVariation = 1;
    public string safeAnimName = "Idle";

    private GameObject inst;
    private Animator anim;
    
    // so i dont have to worry about looping it
    private int currentCount {
        get {return m_currentCount;}
        set {
            if (value > animVariation || value < 1) m_currentCount = 1;
            else m_currentCount = value;
        }
    }

    [SerializeField] private int m_currentCount;

    /// <summery> basic functions to spawn in the weapon <summery>
    public override void unLoad(playerController character) {Destroy(inst);}
    public override void load(playerController character, bool reload = true) {
        canShoot = false;
        character.StartCoroutine(utils.wait(() => {
            // animators
            character.AttackDisplay.runtimeAnimatorController = character.D_AttackDisplay;

            // data
            canShoot = true;
            currentCount = 1;

            // spawn in weapon
            character.StartCoroutine(spawnDelay(character));
        }, 0.1f));
    }

    /// <summery> the attack, this assumes your weapon will use animations </summery>
    public override void attack(playerController character) {
        if (!canShoot) return;

        if (!anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.StartsWith(safeAnimName)) return;
        anim.Play(currentAttackAnimName());

        // incriment anim count
        currentCount++;
    }

    /// <summery> a util to get the current attack name </summery>
    private string currentAttackAnimName() {
        Debug.Log($"Attack{(currentCount > 1 ? $"[{currentCount}]" : "")}");
        return $"Attack{(currentCount > 1 ? $"[{currentCount}]" : "")}";
    }

    /// <summery> a spawn delay </summery>
    private IEnumerator spawnDelay(playerController character) {
        yield return new WaitForSeconds(0.4f);

        inst = Instantiate(weapon, new Vector3(), Quaternion.identity, character.transform);
        inst.transform.localPosition = new Vector3();
        inst.transform.localRotation = Quaternion.identity;
        
        anim = inst.transform.GetComponent<Animator>();
        canShoot = true;
    }
}