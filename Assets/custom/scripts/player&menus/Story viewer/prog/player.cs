// using UnityEngine;
// using UnityEngine.SceneManagement;

// using System;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Collections;
// using System.Collections.Generic;

// using TMPro;

// #region player
// /// <summery> the character class, one which will respond to the world and stuff </summery>
// public class Character : basic {
//     /// <summery> variables </summery>
//     public string name = ""; // the players name (for high score and stuff)
//     public Vector2 position = new Vector2(); // the position of the player
//     public GameObject player;
//     public game.playerState state; // what state the player is currently in
//     public GameObject subScreen;
//     public weapon baseWeapon = new weapon("fists", 5);

//     /// <summery> stats </summery>
//     public int lostHealth = 0;
//     public int Base_Health = 10;

//     /// <summery> xp datga </summery>
//     public int currentXP = 0;
//     public int level = 0;
//     public int score = 0;

//     public void addXP(int xp) {
//         currentXP += xp;
//         score += xp;

//         if (currentXP > (level + 1.25f) * 100) {
//             level++;
//             currentXP -= (int)((level + 0.25f) * 100);
//             lostHealth = 0;
//             World.Main.display($"leveled up: Lv{level - 1} -> Lv{level}\n base health: {Base_Health++} -> {Base_Health}");
//         }
//     }

//     /// <summery> modifiers </summery>
//     public int M_Health = 0;

//     /// <summery> final stat </summmery>
//     public int F_Health = 0;

//     /// <summery> update </summery>
//     public override void Tick() {}
//     public override void LateTick() {
//         F_Health = M_Health + Base_Health - lostHealth;
//         M_Health = 0;
//     }

//     private game.playerState stateToReturnTo;

//     /// <summery> opens a sub screen </summery> 
//     public void OpenSubscreen(string Text, string extra = "") {
//         if (subScreen != null) return;

//         subScreen = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("subScreen" + extra));
//         subScreen.transform.GetComponent<typeOutText>().type(Text);

//         stateToReturnTo = state;
//         state = game.playerState.inMenu;        
//     }

//     /// <summery> closes the sub screen </summery>
//     public void CloseSubscreen() {
//         if (subScreen.transform.GetComponent<typeOutText>().typing) subScreen.transform.GetComponent<typeOutText>().skip();
//         else {
//             UnityEngine.Object.Destroy(subScreen);

//             state = stateToReturnTo;
//         }
//     }

//     /// <summery> gives the player a choice </summery>
//     private System.Action choiceYes;
//     private System.Action choiceNo;
//     public void OpenChoice(string Text, System.Action yes, System.Action no) {
//         if (subScreen != null) return;

//         choiceYes = yes;
//         choiceNo = no;

//         subScreen = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("subScreen"));
//         subScreen.transform.GetComponent<typeOutText>().type(Text);
//         subScreen.transform.GetComponent<typeOutText>().skip();

//         stateToReturnTo = state;
//         state = game.playerState.choice;  
//     }

//     /// <summery> gets what the player can do </summery>
//     public List<playerAction> getPossibleActions() {
//         switch (state) {
//             case game.playerState.free: default:
//                 return new List<playerAction> {
//                     new playerAction("move", () => {
//                         state = game.playerState.moving;
//                     }),
//                     new playerAction("map", () => {
//                         string finalLog = "";

//                         List<List<room>> localMap = new List<List<room>>(World.Main.map);
//                         localMap.Reverse();

//                         for (int y = 0; y < localMap.Count - 1; y++) {
//                             for (int x = 0; x < localMap[0].Count - 1; x++) {
//                                 finalLog += $"{(localMap[y + 1][x] == null ? "□" : (new Vector2(x, localMap.Count - y - 2) == World.Main.loadedRoom ? "X" : "■"))}";
//                                 // finalLog += $"({x}, {y})";
//                             }

//                             finalLog += "\n";
//                         }

//                         OpenSubscreen(finalLog, "MAP");
//                     }),
//                     new playerAction("stats", () => {
//                         string finalLog = "";

//                         finalLog += $"position: ({position.x}, {position.y})\n";
//                         finalLog += $"name: {name}\n";
//                         finalLog += $"health: {F_Health}\n";
//                         finalLog += $"xp: {currentXP}/{(int)((level + 1.25f) * 100)}\n";
//                         finalLog += $"level: {level}\n";
//                         finalLog += $"score: {score}\n";
//                         finalLog += $"floor: {World.Main.floor}\n";

//                         OpenSubscreen(finalLog);
//                     })
//                 };
//             case game.playerState.moving:
//                 return new List<playerAction> {
//                     new playerAction("North", () => {
//                         if (World.Main.requestRoom(this.position + new Vector2(0, 1), this)) this.position += new Vector2(0, 1);
//                     }),
//                     new playerAction("East", () => {
//                         if (World.Main.requestRoom(this.position + new Vector2(1, 0), this)) this.position += new Vector2(1, 0);
//                     }),
//                     new playerAction("South", () => {
//                         if (World.Main.requestRoom(this.position + new Vector2(0, -1), this)) this.position += new Vector2(0, -1);
//                     }),
//                     new playerAction("West", () => {
//                         if (World.Main.requestRoom(this.position + new Vector2(-1, 0), this)) this.position += new Vector2(-1, 0);
//                     }),                    
//                 };
//             case game.playerState.inMenu: 
//                 return new List<playerAction> {
//                     new playerAction("Continue", () => {
//                         CloseSubscreen();
//                     })
//                 };
//             case game.playerState.battle:
//                 return new List<playerAction> {
//                     new playerAction("Attack", () => {
//                         EnemyBattleController.instance.attack(this);
//                     }),
//                     new playerAction("View Stats", () => {
//                         Enemy target = EnemyBattleController.instance.HL_enemy;
//                         OpenSubscreen($"health: {target.health}");
//                     }),
//                     new playerAction("Check Lore", () => {
//                         Enemy target = EnemyBattleController.instance.HL_enemy;
//                         OpenSubscreen(target.lore);
//                     })
//                 };
//             case game.playerState.blank:
//                 return new List<playerAction>();
//             case game.playerState.choice:
//                 return new List<playerAction> {
//                     new playerAction("option 1", () => {
//                         choiceYes();
//                     }),
//                     new playerAction("option 2", () => {
//                         choiceNo();
//                     }),
//                 };
//         }
//     }

//     /// <summery> what will be called upon death </summery>
//     public void end() {
//         ScreenEffect.instance.run("trans", () => {SceneManager.LoadScene("MainMenu");});
//     }

//     /// <summery> create the character </summery>
//     public Character (string name) {
//         this.name = name;
//         this.position = new Vector2();

//         // spawn the game object relating to the player
//         GameObject playerOBJ = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Player"), new Vector3(), Quaternion.identity);
//         playerOBJ.name = $"{name}";
//         player = playerOBJ;
//     }
// }

// public class playerAction {
//     public string name = "";
//     public System.Action command = () => {Debug.Log("bleh");};
//     public List<playerAction> children;

//     public void run() {
//         command();
//     }

//     public playerAction(string name, System.Action command) {
//         this.name = name;
//         this.command = command;
//     }
// }
// #endregion