using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace stats {
    #region statObject
    /// <summery> a stat object to show its stats, what stats can change, etc etc </summery>
    [System.Serializable]
    public class statObject {
        /// <summery> base variables </summery>
        public float baseDamage = 10f;
        public float baseRange = 25f;
        public float baseSpeed = 1f;
        public float lifeSteal = 0f;
        public float luck = 0f;
        public float healthBoost = 0f;

        /// <summery> upgraded stats </summery>
        public float getDamage(AT_base attack) {
            if(this.upgradeAbilities.Contains(stats.config.upgradeTypes.damage)) return baseDamage + attack.attackData.statData.damageBonus;
            else return baseDamage;
        }

        public float getSpeed(AT_base attack) {
            if(this.upgradeAbilities.Contains(stats.config.upgradeTypes.speed)) return baseSpeed + attack.attackData.statData.speedBonus;
            else return baseSpeed;
        }

        public float getLife(AT_base attack) {
            if(this.upgradeAbilities.Contains(stats.config.upgradeTypes.life)) return lifeSteal + attack.attackData.statData.lifeBonus;
            else return lifeSteal;
        }

        public float getLuck(AT_base attack) {
            if(this.upgradeAbilities.Contains(stats.config.upgradeTypes.luck)) return luck + attack.attackData.statData.luckBonus;
            else return luck;
        }

        public float getHealth(AT_base attack) {
            if(this.upgradeAbilities.Contains(stats.config.upgradeTypes.health)) return healthBoost + attack.attackData.statData.healthBonus;
            else return healthBoost;
        }

        public float getRange(AT_base attack) {
            if(this.upgradeAbilities.Contains(stats.config.upgradeTypes.range)) return baseRange + attack.attackData.statData.rangeBonus;
            else return baseRange;   
        }

        /// <summery> the config </summery>
        public List<stats.config.upgradeTypes> upgradeAbilities = new List<stats.config.upgradeTypes>{
            stats.config.upgradeTypes.damage,
            stats.config.upgradeTypes.speed,
            stats.config.upgradeTypes.life,
            stats.config.upgradeTypes.luck,
            stats.config.upgradeTypes.health,
            stats.config.upgradeTypes.range
        };

        /// <summery> a simple initialisation </summery>
        public statObject() {}
    }

    /// <summery> the stat object, so it can be savced </summery>
    [System.Serializable]
    public class statDataObject {
        public float healthBonus = 0f;
        public float luckBonus = 0f;
        public float lifeBonus = 0f;
        public float speedBonus = 0f;
        public float damageBonus = 0f;
        public float rangeBonus = 0f;
    
        public statDataObject() {}
    }

    /// <summery> some basic config data </summery>
    public static class config {
        public enum upgradeTypes {
            damage,
            speed,
            life,
            luck,
            health,
            range
        }
    }

    #endregion
}