using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using TMPro;

#region Enemys
[System.Serializable]
public class Enemy {
    /// sprites
    public roomTextures textures;

    // public Texture2D self;
    public List<Texture2D> self = new List<Texture2D>();

    public Color col;
    public Color S_col;

    // data
    public int maxHealth = 10;
    public int health = 10;
    public string lore = "";
    public string name = "";
    public weapon A_weapon = new weapon("howd you get this", 0);

    public int xp = 0;
    public int dropChance = 0;

    public int attack() {
        return A_weapon.enemyAttack();
    }

    public void die() {
        World.Main.player[0].addXP(xp);
        World.Main.display($"gained {xp}xp");

        if(UnityEngine.Random.Range(0, 100) <= dropChance) {
            int damageDiff = World.Main.player[0].baseWeapon.damage - A_weapon.damage;

            World.Main.player[0].OpenSubscreen(
                $"{name} has dropped {A_weapon.name}, Would you take it? ({(damageDiff >= 0 ? "-" : "+")}{(Mathf.Abs(damageDiff))}) or heal {(int)(maxHealth / 10)} points?",
                new task(
                    "weapon pickup",
                    new List<taskAction> {
                        new taskAction(
                            $"Take {A_weapon.name}",
                            (task self) => {
                                World.Main.display($"swapped: {World.Main.player[0].baseWeapon.name} for {A_weapon.name}");
                                World.Main.player[0].baseWeapon = A_weapon;
                                UnityEngine.Object.Destroy(World.Main.player[0].subScreen);
                                World.Main.player[0].state = game.playerState.free;
                                World.Main.player[0].tasks.completeCurrentTask();
                            }
                        ),
                        new taskAction(
                            $"heal {(int)(maxHealth / 10)}",
                            (task self) => {
                                World.Main.display($"healed {(int)(maxHealth / 10)} points");
                                World.Main.player[0].lostHealth -= (int)(maxHealth / 10);
                                if (World.Main.player[0].lostHealth < 0) World.Main.player[0].lostHealth = 0;
                                UnityEngine.Object.Destroy(World.Main.player[0].subScreen);
                                World.Main.player[0].state = game.playerState.free;
                                World.Main.player[0].tasks.completeCurrentTask();
                            }
                        ),
                    }
                )
            );
        }
    }

    public Enemy copy() {
        return new Enemy(
            A_weapon,
            col,
            S_col,
            name,
            
            textures,

            self,
            maxHealth,
            xp,
            lore,
            dropChance
        );
    }

    /*
        - an enemy created using strings
            - plus only one enemy sprite
    */
    public Enemy(weapon A_weapon,Color col,Color S_col,string name,roomTextures textures,string self = "",int maxHealth = 10,int xp = 10,string lore = "",int dropChance = 0) {
        this.col = col;
        this.S_col = S_col;
        this.name = name;

        this.textures = textures;

        this.self = new List<Texture2D>(){
            Resources.Load<Texture2D>(self)
        };

        this.maxHealth = maxHealth;
        this.xp = xp;
        this.lore = lore;
        this.A_weapon = A_weapon;
        this.dropChance = dropChance;

        this.health = maxHealth;
    }

    /*
        - an enemy created using Texture2Ds
            - plus only one enemy sprite
    */
    public Enemy(weapon A_weapon, Color col, Color S_col, string name, roomTextures textures,Texture2D self,int maxHealth = 10,int xp = 10, string lore = "", int dropChance = 0 ) {
        this.col = col;
        this.S_col = S_col;
        this.name = name;

        this.textures = textures;

        this.self = new List<Texture2D> {
            self
        };

        this.maxHealth = maxHealth;
        this.xp = xp;
        this.lore = lore;
        this.A_weapon = A_weapon;
        this.dropChance = dropChance;

        this.health = maxHealth;
    }

    /*
        - an enemy created using strings
            - plus multiple enemy sprites
    */
    public Enemy(weapon A_weapon,Color col,Color S_col,string name,roomTextures textures,List<string> self,int maxHealth = 10,int xp = 10,string lore = "",int dropChance = 0) {
        this.col = col;
        this.S_col = S_col;
        this.name = name;

        this.textures = textures;

        this.self = new List<Texture2D>();
        foreach(string frame in self) this.self.Add(Resources.Load<Texture2D>(frame));

        this.maxHealth = maxHealth;
        this.xp = xp;
        this.lore = lore;
        this.A_weapon = A_weapon;
        this.dropChance = dropChance;

        this.health = maxHealth;
    }

    /*
        - an enemy created using Texture2Ds
            - plus multiple enemy sprite
    */
    public Enemy(weapon A_weapon, Color col, Color S_col, string name, roomTextures textures,List<Texture2D> self,int maxHealth = 10,int xp = 10, string lore = "", int dropChance = 0 ) {
        this.col = col;
        this.S_col = S_col;
        this.name = name;

        this.textures = textures;

        this.self = self;

        this.maxHealth = maxHealth;
        this.xp = xp;
        this.lore = lore;
        this.A_weapon = A_weapon;
        this.dropChance = dropChance;

        this.health = maxHealth;
    }
}
#endregion