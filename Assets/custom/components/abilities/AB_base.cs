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
        public virtual void start(playerController character) {}

        /// <summery> code ran every frame </summery>
        public virtual void update(playerController character) {}

        /// <summery> code ran on the end of the scene </summery>
        public virtual void end(playerController character) {}
        
        /// <summery> the main ability </summery>
        public virtual void use(playerController character) {
            // cost
            if (character.attack.liveKills < cost) return;
            character.attack.liveKills -= cost;
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