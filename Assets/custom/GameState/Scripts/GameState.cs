using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState")]
public class GameState : ScriptableObject {
    public bool paused = false;
    public bool helped = false;
    public bool menued = false;
    public bool loaded = false;

    public GameObject player;

    public List<attack.attackRegistration> registeredAttacks;

    public roomData.map map;

    public void pause(bool pauseSet) {
        Time.timeScale = pauseSet ? 0 : 1;
        paused = pauseSet;

        // Cursor.lockState = pauseSet ? CursorLockMode.None : CursorLockMode.Locked;
        // Cursor.visible = pauseSet;
    }

    public void help(bool helpSet) {
        Time.timeScale = helpSet ? 0 : 1;
        helped = helpSet;

        Cursor.lockState = (helpSet ? CursorLockMode.None : CursorLockMode.Locked);
        Cursor.visible = helpSet;
    }

    public void menu(bool menuSet) {
        Time.timeScale = menuSet ? 0 : 1;
        menued = menuSet;

        Cursor.lockState = (menuSet ? CursorLockMode.None : CursorLockMode.Locked);
        Cursor.visible = menuSet;
    }

    public AT_base getCurrentAttack(string name) {
        foreach (attack.attackRegistration attack in registeredAttacks) if (attack.name == name) return attack.attack;
        
        return null;
    }
}
