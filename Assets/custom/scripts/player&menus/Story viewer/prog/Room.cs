using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using sys;

#region rooms
[System.Serializable]
public class room {
    /// sprites
    public roomTextures textures;

    // directions
    public List<string> inDir = new List<string>();
    public List<string> outDir = new List<string>();

    // dimensions
    public Vector3 upperBounds = new Vector3(5, 5, 5); // top front left
    public Vector3 lowerBounds = new Vector3(-5, 0, -5); // bottom back right
    public Vector3 center => new Vector3(
        (upperBounds.x + lowerBounds.x) / 2,
        (upperBounds.y + lowerBounds.y) / 2,
        (upperBounds.z + lowerBounds.z) / 2
    ); // the center point
    public Vector3 offset = new Vector3(0, 0, 2.5f); // a little offset
    public int enterCount = 0;

    public game.roomType type = game.roomType.defined;
    public roomEvent S_event = new roomEvent();
    public Color col = Color.red;
    public Color S_col = Color.black;

    /// <summery> load the room </summery>
    public GameObject generate() {
        GameObject room = new GameObject(); // make the room
        room.name = "room";
        colorManager.data.targetColor = col;
        colorManager.data.secondColor = S_col;

        roomTextureCollection tex = textures.getTextures();

        // make the floor
        renderWall(
            tex.floor,
            new Vector3(this.center.x, 0, this.center.z),
            new Vector3(90, 0, 0),
            "floor"
        ).transform.parent = room.transform;

        // make the roof
        renderWall(
            tex.roof,
            new Vector3(this.center.x, this.upperBounds.y, this.center.z),
            new Vector3(90, 0, 0),
            "roof"
        ).transform.parent = room.transform;

        // North
        renderWall(
            tex.north,
            new Vector3(0, this.center.y, this.upperBounds.z),
            new Vector3(0, 0, 0),
            "North"
        ).transform.parent = room.transform;

        // East
        renderWall(
            tex.east,
            new Vector3(-this.upperBounds.x, this.center.y, 0),
            new Vector3(0, 90, 0),
            "East"
        ).transform.parent = room.transform;

        // South
        renderWall(
            tex.south,
            new Vector3(0, this.center.y, -this.upperBounds.z),
            new Vector3(0, 0, 0),
            "South"
        ).transform.parent = room.transform;

        // West
        renderWall(
            tex.west,
            new Vector3(this.upperBounds.x, this.center.y, 0),
            new Vector3(0, 90, 0),
            "West"
        ).transform.parent = room.transform;

        // by default offset the room by a lil
        room.transform.position += offset;

        if (S_event != null) {
            if (enterCount == 0) S_event.OnFirstEnter();
            else S_event.OnAnotherEnder(enterCount);
        }
        enterCount++;

        return room;
    }
    /// <summery> a thing to let me copy </summery>
    public room copy() {
        return new room(
            upperBounds,
            lowerBounds,
            offset,
            inDir,
            outDir,
            col,
            S_col,
            textures,
            S_event != null ? S_event.copy() : null
        );
    }

    /// <summery> a helper function to make the walls </summery>
    private GameObject renderWall(Texture2D Tex, Vector3 pos, Vector3 angles, string name = "") {
        GameObject wall = new GameObject();
        wall.name = name;
        if (Tex == null) return wall;

        wall.AddComponent<SpriteRenderer>().sprite = Sprite.Create(Tex, new Rect(0.0f, 0.0f, Tex.width, Tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        wall.transform.position = pos;
        wall.transform.eulerAngles = angles;
        return wall;
    }

    // /// <summery> get a texture from a file instead </summery>
    // public Texture2D returnFromFile() {

    // }

    /// <summery> create a basic room </summery>
    /// is this bad, yes, do i care, no
    public room (Vector3 UB, Vector3 LB,Vector3 Offset,List<string> inDir,List<string> outDir,Color col,Color S_col,roomTextures textures,roomEvent S_event = null) {
        this.upperBounds = UB;
        this.lowerBounds = LB;
        this.offset = Offset;
        this.inDir = inDir;
        this.outDir = outDir;

        this.textures = textures;

        this.S_event = S_event;
        this.col = col;
        this.S_col = S_col;
    }
}

/// <summery> this could be a pickup or start a battle, etc </summery>
public class roomEvent {
    public virtual void OnFirstEnter() {
        World.Main.display("you activated my room event");
    }

    public virtual void OnAnotherEnder(int amount) {
        World.Main.display($"you have been in this room {amount} times");
    }

    public virtual roomEvent copy() {
        return new roomEvent();
    }

    public roomEvent() {}
}

public class EnemyRoomEvent : roomEvent {
    private Enemy en;

    public override void OnFirstEnter() {
        EnemyBattleController.instance.StartBattle(en);
    }

    public override roomEvent copy() {
        return new EnemyRoomEvent(en.copy());
    }

    public EnemyRoomEvent(Enemy en) {
        this.en = en;
    }
}

public class ItemRoomEvent : roomEvent {
    private item Item;

    public override void OnFirstEnter() {
        World.Main.player[0].OpenSubscreen(
            $"would you like to pick up {Item.name}",
            new task (
                $"{Item.name} pickup menu",
                new List<taskAction> {
                    new taskAction(
                        $"pick up {Item.name}",
                        (task self) => {
                            World.Main.player[0].inv.addItem(Item);
                            self.complete();
                        }
                    ),
                    new taskAction(
                        $"dont pick up {Item.name}",
                        (task self) => {
                            self.complete();
                        }
                    )
                }
            )
        );
    }

    public override roomEvent copy() {
        return new ItemRoomEvent(Item.copy());
    }

    public ItemRoomEvent(item Item) {
        this.Item = Item;
    }
}

public class StairsRoomEvent : roomEvent {
    public override void OnFirstEnter() {
        this.OnAnotherEnder(0);
    }

    public override void OnAnotherEnder(int amount) {
        World.Main.player[0].OpenSubscreen(
            "would you like to advance to the next floor?",
            new task(
                "stair event",
                new List<taskAction> {
                    new taskAction(
                        "yes",
                        (task self) => {
                            self.complete();
                            
                            ScreenEffect.instance.run("trans", () => {
                                World.Main.descendFloor();
                            });
                        }
                    ),
                    new taskAction(
                        "no",
                        (task self) => {
                            World.Main.display("you can reload the room to go back");
                            World.Main.player[0].tasks.completeCurrentTask();
                        }
                    ),
                }
            )
        );
    }

    public override roomEvent copy() {return new StairsRoomEvent();}
    public StairsRoomEvent() {}
}

public class roomTextureCollection {
    public Texture2D roof;
    public Texture2D floor;
    public Texture2D east;
    public Texture2D west;
    public Texture2D north;
    public Texture2D south;

    public roomTextureCollection(Texture2D roof, Texture2D floor, Texture2D east, Texture2D west, Texture2D north, Texture2D south) {
        this.roof = roof;
        this.floor = floor;
        this.east = east;
        this.west = west;
        this.north = north;
        this.south = south;
    }
}

public class roomTextures {
    // data
    private List<Texture2D> roof;
    private List<Texture2D> floor;
    private List<Texture2D> east;
    private List<Texture2D> west;
    private List<Texture2D> north;
    private List<Texture2D> south;

    private int index = 1;

    public roomTextureCollection getTextures() {

//         Debug.Log(@$"
// roof: {roof.Count} => index :: {roof.wrapIndex(index)}
// floor: {floor.Count} => index :: {floor.wrapIndex(index)}
// east: {east.Count} => index :: {east.wrapIndex(index)}
// west: {west.Count} => index :: {west.wrapIndex(index)}
// north: {north.Count} => index :: {north.wrapIndex(index)}
// south: {south.Count} => index :: {south.wrapIndex(index)}
//         ");

        roomTextureCollection item = new roomTextureCollection(
            roof[roof.wrapIndex(index)],
            floor[floor.wrapIndex(index)],
            east[east.wrapIndex(index)],
            west[west.wrapIndex(index)],
            north[north.wrapIndex(index)],
            south[south.wrapIndex(index)]
        );

        index++;

        return item;
    }

    // a huge amount of constructors
    public roomTextures(Texture2D roof, Texture2D floor, Texture2D east, Texture2D west, Texture2D north, Texture2D south) {
        this.roof = new List<Texture2D>{roof};
        this.floor = new List<Texture2D>{floor};
        this.east = new List<Texture2D>{east};
        this.west = new List<Texture2D>{west};
        this.north = new List<Texture2D>{north};
        this.south = new List<Texture2D>{south};

        index = 1;
    }

    public roomTextures(List<Texture2D> roof, List<Texture2D> floor, List<Texture2D> east, List<Texture2D> west, List<Texture2D> north, List<Texture2D> south) {
        this.roof = roof;
        this.floor = floor;
        this.east = east;
        this.west = west;
        this.north = north;
        this.south = south;

        index = 1;
    }
}
#endregion