using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "new ability", menuName = "ability/base")]
public class AB_base : ScriptableObject {
    [Header("config")]
    public int cost = 5;

    /// <summery> the main functions </summery>
    #region Main    
        /// <summery> the start function, use to load values etc </summery>
        public virtual void start(playerController character) {Debug.Log("loaded");}

        /// <summery> code ran every frame </summery>
        public virtual void update(playerController character) {Debug.Log("update");}

        /// <summery> code ran on the end of the scene </summery>
        public virtual void end(playerController character) {Debug.Log("ended");}
        
        /// <summery> the main ability </summery>
        public virtual void use(playerController character) {
            // cost
            if (character.attack.liveKills < cost) return;
            character.attack.liveKills -= cost;

            Debug.Log("used ability");
        }
    #endregion
}

namespace ability {
    [System.Serializable]
    public class abilityRegistraction {
        public string name;
        public AB_base ability;
    }
}