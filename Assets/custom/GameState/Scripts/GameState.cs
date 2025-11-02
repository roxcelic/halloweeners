using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject {
    public bool paused = false;
    public bool loaded = false;

    public GameObject player;

    public List<attack.attackRegistration> registeredAttacks;

    public void pause(bool pauseSet) {
        Time.timeScale = pauseSet ? 0 : 1;
        paused = pauseSet;

        // Cursor.lockState = pauseSet ? CursorLockMode.None : CursorLockMode.Locked;
        // Cursor.visible = pauseSet;
    }

    public AT_base getCurrentAttack(string name, attack.attackData data) {
        foreach (attack.attackRegistration attack in registeredAttacks) {
            if (attack.name == name) {
                attack.attack = attack.attack.protection();
                attack.attack.attackData = data;

                return attack.attack;
            }
        }
        
        return null;
    }
}
