using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        // animators
        character.AttackDisplay.runtimeAnimatorController = null;

        // data
        canShoot = true;
        currentCount = 1;

        // spawn in weapon
        inst = Instantiate(weapon, new Vector3(), Quaternion.identity, character.transform);
        anim = inst.transform.GetComponent<Animator>();
    }

    /// <summery> the attack, this assumes your weapon will use animations </summery>
    public override void attack(playerController character) {
        if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != safeAnimName) return;
        anim.Play(currentAttackAnimName());

        // incriment anim count
        currentCount++;
    }

    /// <summery> a util to get the current attack name </summery>
    private string currentAttackAnimName() {
        return $"Attack{(currentCount > 1 ? $"[{currentCount}]" : "")}";
    }
}