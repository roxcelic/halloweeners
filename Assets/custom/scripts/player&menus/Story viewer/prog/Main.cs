using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using TMPro;

#region weapons 
// this will allow you to attack the enemys
[System.Serializable]
public class weapon {
    public string name = "";
    public int damage = 0;
    public int enemyDamage = 0;

    public List<taskAction> attackData;

    public int attack() {
        return damage;
    }

    public int enemyAttack() {
        return enemyDamage;
    }

    public taskAction expandAttack(System.Action<task> exp, int index = 0) {
        return new taskAction(
            attackData[index].name,
            attackData[index].act + exp
        );
    }

    public weapon(string name, int damage, int enemyDamage = 0, List<taskAction> attackData = null) {
        this.name = name;
        this.damage = damage;
        this.enemyDamage = enemyDamage == 0 ? damage : enemyDamage;
        if (attackData != null) this.attackData = attackData;
        else {
            this.attackData = new List<taskAction>{new taskAction(
                "hit",
                (task self) => {
                    EnemyBattleController.instance.attack(damage);
                    self.complete();
                }
            )};
        }
    }
}
#endregion

#region game
public static class game {
    public static World world; // the world
    public static List<string> log = new List<string>(); // the games log

    /// <summery> enums </summery>
    public enum roomType {
        defined,
        local
    }

    public enum playerState {
        free,
        moving,
        inMenu,
        battle,
        blank,
        choice
    }

    public static async void Begin() {
        world = new World("");    
        world.display("starting");
        await world.Generate(10, 10);
        world.display("made player");
        Character2 me = new Character2("Roxy");
        world.Add(me);
        world.display("world start");
        world.StartSimulation();
    }
}
#endregion