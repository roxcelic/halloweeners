using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using TMPro;

#region world
/// <summery> This is the main class for the game </summery>
/// since im aiming for a much simpler game this will be created within the character or something
///     It will hold the game stats
///     have a variable to hold a collection of actions when an update occurs
///     Etc Etc
/// This is in global space because fuck you
[System.Serializable]
public class World {
    /// <summery> allows everything to refrence this object </summery>
    public static World Main;

    /// <summery> variables </summery>
    public readonly string seed = "";
    public List<System.Action> actions = new List<System.Action>(); // holds the list of actions to be called when updated
    public Vector2 position = new Vector2(); // the current position of the character
    public List<Character2> player = new List<Character2>(); // the characters
    public int tickSpeed = 50; // how many times a tick is called
    public List<List<room>> map= new List<List<room>>();
    public GameObject currentRoom; // the current room
    public int floor = 1;

    public Vector2 loadedRoom;

    private Vector2 startPos = new Vector2();

    /// <summery> add the character to the world </summery>
    public void Add(Character2 me) {
        this.player.Add(me);
    }

    /// <summery> used to get / set a room in a position, also checks to see if the player can move to that position </summery>
    public bool requestRoom(Vector2 pos, Character2 me, bool movement = true) {
        if (!this.player.Contains(me)) throw new InvalidOperationException("this is an unregistered player"); // if the player is not registered
        
        if (movement && (Mathf.Abs(me.position.x - pos.x) > 1 || Mathf.Abs(me.position.y - pos.y) > 1)) {
            display("not within appropriate movement");
            return false; // is this within movement distance
        }

        if (pos.y > map.Count - 1 || pos.y < 0 || pos.x > map[0].Count - 1 || pos.x < 0) {
            display("you are attempting to move out of bounds");
            return false; // is within bounds
        }

        if (!map[(int)loadedRoom.y][(int)loadedRoom.x].outDir.Contains(roomCollection.data.Vector2ToDir(new Vector2(Mathf.Abs(me.position.x - pos.x), Mathf.Abs(me.position.y - pos.y))))) {
            display("this direction is blocked");
            return false;
        }

        if (map[(int)pos.y][(int)pos.x] != null) {
            if (!map[(int)pos.y][(int)pos.x].inDir.Contains(roomCollection.data.Vector2ToDir(new Vector2(Mathf.Abs(me.position.x - pos.x), Mathf.Abs(me.position.y - pos.y))))) {
                display("this direction is blocked");
                return false;
            }

            player[0].state = game.playerState.blank;
            ScreenEffect.instance.run("trans", () => {
                player[0].state = game.playerState.free;
                UnityEngine.Object.Destroy(this.currentRoom);
                currentRoom = map[(int)pos.y][(int)pos.x].generate();
                loadedRoom = new Vector2(pos.x, pos.y);
            });

            return true;
        } // if the room exists

        List<string> mustHave = new List<string> {roomCollection.data.Vector2ToDir(new Vector2(
            Mathf.Abs(me.position.x - pos.x), Mathf.Abs(me.position.y - pos.y)))};

        List<string> cantHave = new List<string>();
        if (pos.y == map.Count - 2) cantHave.Add("North");
        if (pos.y == 0) cantHave.Add("South");
        if (pos.x == map[0].Count - 2) cantHave.Add("East");
        if (pos.x == 0) cantHave.Add("West");

        List<room> possibleRooms = roomCollection.data.findPossibleRooms(mustHave, cantHave, floor);
        if (possibleRooms.Count == 0) {
            display("no possible rooms can be generated");
            return false;
        }

        int selectedIndex = UnityEngine.Random.Range(0, possibleRooms.Count);
        display(selectedIndex.ToString());
        room selectedRoom = possibleRooms[selectedIndex];

        player[0].state = game.playerState.blank;
        ScreenEffect.instance.run("trans", () => {
            player[0].state = game.playerState.free;
            UnityEngine.Object.Destroy(this.currentRoom);
            map[(int)pos.y][(int)pos.x] = selectedRoom.copy();
            currentRoom = map[(int)pos.y][(int)pos.x].generate();
            loadedRoom = new Vector2(pos.x, pos.y);
        });

        return true;
    }
    
    /// <summmery> allows displaying some more info to the player </summery>
    public void display(string Text) {
        GameObject logItem = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LogItem"));
        logItem.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = Text;
    }

    /// <summery> end the game </summery>
    public void execute(Character2 me) {
        me.end();
        this.player.Remove(me);
    } // unfinished, i just wanted to refrence https://youtu.be/ESx_hy1n7HA

    /// <summery> begin tick management etc etc
    public async void StartSimulation() {
        if (this.player.Count == 0) throw new InvalidOperationException("no, players cannot start");

        // set the players start pos
        Debug.Log($"starting player at {startPos}");
        foreach(Character2 PL in this.player) PL.position = startPos;

        while (true) {
            // basic update
            List<basic> updatedItems= new List<basic>();
            foreach(basic ent in basic.entities) {
                if (ent != null) {
                    ent.Tick();
                    updatedItems.Add(ent);
                }
            }
            basic.entities = updatedItems;

            await Task.Delay(tickSpeed);
            // late update
            updatedItems= new List<basic>();
            foreach(basic ent in basic.entities) {
                if (ent != null) {
                    ent.LateTick();
                    updatedItems.Add(ent);
                }
            }
            basic.entities = updatedItems;
        }
    }
    
    /// <summery> move the game down 1 floor </summery>
    public async void descendFloor() {
        floor++;
        Vector2 worldSize = new Vector2(map.Count, map[0].Count);
        map = new List<List<room>>();
        await Generate((int)worldSize.x, (int)worldSize.y);
        foreach(Character2 PL in this.player) PL.position = startPos;
    }
    
    /// <summery> generate the world </summmery>
    public async Task Generate(int width, int height) {
        // populate map
        for (int y = 0; y < height; y++) {
            this.map.Add(new List<room>());
            for (int x = 0; x < width; x++) {
                this.map[y].Add(null);
                await Task.Delay(0); // a delay used for testing but i might come back to
            } 
        }

        this.startPos = new Vector2((width / 2) - 1, (height / 2) - 1);

        // set start room
        this.map[(int)startPos.y][(int)startPos.x] = new room(
            new Vector3(2.5f, 2.5f, 2.5f), // upper bounds
            new Vector3(-2.5f, 0, -2.5f), // lower bounds
            new Vector3(0, 0, 2.5f), // offset
            new List<string>() {"North"}, // in directions
            new List<string>() {"North"}, // out directions directions
            Color.red,
            Color.black,
            new roomTextures(
                Resources.Load<Texture2D>("rooms/brick/roof"),
                Resources.Load<Texture2D>("rooms/brick/floor"),
                Resources.Load<Texture2D>("rooms/brick/wall"),
                Resources.Load<Texture2D>("rooms/brick/wall"),
                Resources.Load<Texture2D>("rooms/brick/wall"),
                Resources.Load<Texture2D>("rooms/brick/wall")
            )
        );

        this.currentRoom = this.map[(int)startPos.y][(int)startPos.x].generate();
        loadedRoom = startPos;
    }

    /// <summery> create the world </summery>
    public World(string seed) {
        this.seed = seed;
        World.Main = this;
    }
}
#endregion