using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summery> an item class </summery>
public class item {
    public string name = "";
    public string description = "";
    public int amount = 1;
    public System.Action use;

    public void play() {
        amount--;
        use();
    }

    public item copy() {
        return new item (
            name,
            description,
            use
        );
    }

    public item (
        string name,
        string description,
        System.Action use
    ) {
        this.name = name;
        this.description = description;
        this.use = use;
    }
}

/// <summery> an inventory class </summery>
public class inventory {
    public int maxItems = 8;
    public List<item> container = new List<item>();

    public bool addItem(item Item) {
        item foundItem;
        if ((foundItem = container.FirstOrDefault(s => s.name == Item.name)) != null) foundItem.amount++;
        else {
            if (container.Count > maxItems) return false;

            container.Add(Item);
        }
        return true;
    }

    public bool useItem(string name) {
        item foundItem;
        if ((foundItem = container.Single(s => s.name == name)) != null) {
            foundItem.play();
            World.Main.display($"used {name}, {foundItem.amount} left");
            if(foundItem.amount <= 0) container.Remove(foundItem);
        } else return false;
        return true;
    }

    public task useItems(bool backButton = true) {
        task finalTask = new task(
            "item use menu",
            new List<taskAction>()
        );

        foreach (item Item in container) {
            finalTask.actions.Add(
                new taskAction (
                    $"{Item.name} x{Item.amount}" ,
                    (task self) => {
                        task beep = World.Main.player[0].tasks.addTask(
                            new task (
                                $"{Item.name} use menu",
                                new List<taskAction> {
                                    new taskAction(
                                        "use",
                                        (task selfcest) => {
                                            World.Main.player[0].inv.useItem(Item.name);
                                            selfcest.complete();
                                            self.complete();
                                        }
                                    ),
                                    new taskAction(
                                        "description",
                                        (task selfcest) => {

                                            task descTask = World.Main.player[0].OpenSubscreen(
                                                Item.description,
                                                new task(
                                                    $"{Item.name} description menu",
                                                    new List<taskAction> {
                                                        new taskAction(
                                                            "continue",
                                                            (task selfcest2) => {
                                                                selfcest2.complete();
                                                            }
                                                        )
                                                    }
                                                )
                                            );

                                            World.Main.player[0].tasks.prioritiseTask(descTask);
                                        }
                                    ),
                                    new taskAction(
                                        "back",
                                        (task selfcest) => {
                                            selfcest.complete();
                                        }
                                    )
                                }
                            )
                        );

                        World.Main.player[0].tasks.prioritiseTask(beep);
                    }
                )
            );
        }

        if(backButton || finalTask.actions.Count == 0) finalTask.actions.Add(
            new taskAction(
                "close",
                (task self) => {
                    self.complete();
                }
            )
        );

        return finalTask;
    }

    public inventory() {}
}