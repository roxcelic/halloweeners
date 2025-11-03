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

    public AT_base getCurrentAttack(string name) {
        foreach (attack.attackRegistration attack in registeredAttacks) if (attack.name == name) return attack.attack;
        
        return null;
    }
}
