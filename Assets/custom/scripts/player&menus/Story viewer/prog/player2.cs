using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using TMPro;

/// The second attempt at a player class, this time i want to have it work like a task system
public class Character2 : basic {
    // the task collection
    public taskCollection tasks = new taskCollection(
        new task (
            "///",
            new List<taskAction> { 
                new taskAction(
                    "Move",
                    (task self) => {
                        World.Main.player[0].tasks.addTask(
                            "movement menu",
                            new List<taskAction> {
                                new taskAction(
                                    "North",
                                    (task self) => {
                                        if (World.Main.requestRoom(World.Main.player[0].position + new Vector2(0, 1), World.Main.player[0])) World.Main.player[0].position += new Vector2(0, 1);
                                        self.complete();
                                    }
                                ),
                                new taskAction(
                                    "East",
                                    (task self) => {
                                        if (World.Main.requestRoom(World.Main.player[0].position + new Vector2(1, 0), World.Main.player[0])) World.Main.player[0].position += new Vector2(1, 0);
                                        self.complete();
                                    }
                                ),
                                new taskAction(
                                    "South",
                                    (task self) => {
                                        if (World.Main.requestRoom(World.Main.player[0].position + new Vector2(0, -1), World.Main.player[0])) World.Main.player[0].position += new Vector2(0, -1);
                                        self.complete();
                                    }
                                ),
                                new taskAction(
                                    "West",
                                    (task self) => {
                                        if (World.Main.requestRoom(World.Main.player[0].position + new Vector2(-1, 0), World.Main.player[0])) World.Main.player[0].position += new Vector2(-1, 0);
                                        self.complete();
                                    }
                                ),
                                new taskAction(
                                    "Back",
                                    (task self) => {
                                        self.complete();
                                    }
                                )
                            }
                        );
                    }
                ),
                new taskAction(
                    "open map",
                    (task self) => {
                        string finalLog = "";

                        List<List<room>> localMap = new List<List<room>>(World.Main.map);
                        localMap.Reverse();

                        for (int y = 0; y < localMap.Count - 1; y++) {
                            for (int x = 0; x < localMap[0].Count - 1; x++) {
                                finalLog += $"{(localMap[y + 1][x] == null ? "□" : (new Vector2(x, localMap.Count - y - 2) == World.Main.loadedRoom ? "X" : "■"))}";
                            }

                            finalLog += "\n";
                        }

                        World.Main.player[0].OpenSubscreen(
                            finalLog,
                            new task(
                                "map",
                                new List<taskAction> {
                                    new taskAction (
                                        "continue",
                                        (task self) => {
                                            self.complete();
                                        }
                                    )
                                }
                            ),
                            true,
                            "MAP"
                        );
                        self.complete();
                    }
                ),
                new taskAction(
                    "open inventory",
                    (task self) => {
                        World.Main.player[0].tasks.addTask(World.Main.player[0].inv.useItems());
                        self.complete();
                    }
                )
            }
        )
    );

    public game.playerState state; // temporary

    /// <summery> variables </summery>
    public string name = ""; // the players name (for high score and stuff)
    public Vector2 position = new Vector2(); // the position of the player
    public GameObject player;
    public GameObject subScreen;
    public weapon baseWeapon = new weapon("fists", 5);

    /// <summery> stats </summery>
    public int lostHealth = 0;
    public int Base_Health = 10;

    /// <summery> inventory </summery>
    public inventory inv = new inventory();

    /// <summery> xp data </summery>
    public int currentXP = 0;
    public int level = 0;
    public int score = 0;

    public void addXP(int xp) {
        currentXP += xp;
        score += xp;

        if (currentXP >= (level + 1.25f) * 100) {
            level++;
            currentXP -= (int)((level + 0.25f) * 100);
            lostHealth = 0;
            OpenSubscreen(
                $"leveled up: Lv{level - 1} -> Lv{level}\n base health: {Base_Health++} -> {Base_Health}",
                new task(
                    "level up screen",
                    new List<taskAction> {
                        new taskAction(
                            "continue",
                            (task self) => {
                                self.complete();
                            }
                        )
                    }
                )
            );
        }
    }

    /// <summery> modifiers </summery>
    public int M_Health = 0;

    /// <summery> final stat </summmery>
    public int F_Health = 0;

    /// <summery> update </summery>
    public override void Tick() {}
    public override void LateTick() {
        F_Health = M_Health + Base_Health - lostHealth;
        M_Health = 0;
    }
    
    /// <summery> opens a sub screen </summery> 
    public task OpenSubscreen(string Text, task taskData, bool quick = true, string extra = "") {
        if (subScreen != null) UnityEngine.Object.Destroy(subScreen);

        taskData.indulgeTask(
            () => { // on task start
                subScreen = UnityEngine.Object.Instantiate(Resources.Load<GameObject>($"subScreen{extra}"));
                subScreen.transform.GetComponent<typeOutText>().type(Text);

                if (quick) subScreen.transform.GetComponent<typeOutText>().skip();
            },
            () => { // on task end
                UnityEngine.Object.Destroy(subScreen);
            }
        );

        tasks.addTask(taskData);

        return taskData;
    }

    /// <summery> what will be called upon death </summery>
    public void end() {
        ScreenEffect.instance.run("trans", () => {SceneManager.LoadScene("MainMenu");});
    }

    /// <summery> create the character </summery>
    public Character2 (string name) {
        this.name = name;
        this.position = new Vector2();

        // spawn the game object relating to the player
        GameObject playerOBJ = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Player"), new Vector3(), Quaternion.identity);
        playerOBJ.name = $"{name}";
        player = playerOBJ;
    }
}

/// <summery> this will give the player an ability from that task </summery>
public class taskAction {
    public string name = "";
    public System.Action<task> act;

    public taskAction(string name, System.Action<task> act) {
        this.name = name;
        this.act = act;
    }
}

/// <summery> a basic task </summery>
public class task {
    private int discovered = 0;
    public bool completed = false;

    public string name = "";
    public List<taskAction> actions = new List<taskAction>();

    public System.Action onTaskStart = null;
    public System.Action onTaskEnd = null;

    public void complete() {
        completed = true;
        if (onTaskEnd != null) onTaskEnd();
    }

    public void onDiscover() {
        if (discovered == 0 && onTaskStart != null) onTaskStart();
        discovered++;
    }

    public void indulgeTask(System.Action onStart, System.Action onEnd = null) {
        onTaskStart = onStart;
        onTaskEnd = onEnd;
    }

    public task(string name, List<taskAction> actions) {
        this.name = name;
        this.actions = actions;
    }
}

/// <summery> a collection of tasks to tell the player what to do next </summery>
public class taskCollection {
    public List<task> tasks = new List<task>();
    public task baseTask = new task("///", new List<taskAction>{
        new taskAction("///", (task self) => {World.Main.display("how the fuck??");})
    });

    public void completeCurrentTask() {this.nextTask().complete();}

    public task addTask(string name, List<taskAction> actions) {
        task newTask = new task(name, actions);
        tasks.Add(newTask);
        return newTask;
    }

    public task addTask(task taskData) {
        tasks.Add(taskData);
        return taskData;
    }

    public void prioritiseTask(task taskData) {
        if(!tasks.Contains(taskData)) throw new InvalidOperationException("task is not in task collection");
        int taskIndex = tasks.FindIndex(a => a == taskData);

        tasks.RemoveAt(taskIndex);
        tasks.Insert(0, taskData);
    }

    public task nextTask() {
        List<task> modifiedTaskList = new List<task>(tasks);

        foreach (task T_task in tasks) {
            if (T_task.completed) modifiedTaskList.Remove(T_task);
            else {
                tasks = modifiedTaskList;
                T_task.onDiscover();
                return T_task;
            }
        }

        tasks = modifiedTaskList;
        return baseTask;
    }

    public string displayTasks() {
        string finalText = "";

        foreach(task T_task in tasks) finalText += $"\n\t-{T_task.name}";

        return finalText;
    }

    public taskCollection(task basetTask) {
        this.baseTask = basetTask;
    }
}