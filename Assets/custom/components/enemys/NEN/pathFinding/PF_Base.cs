using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ext;

namespace pathFinding {

    /*  VISUALISATION
        [0],[0],[0],[0],[0],[0],[0],[0],[0]
        [0],[0],[0],[0],[0],[0],[0],[0],[0]
        [0],[0],[0],[0],[0],[0],[0],[0],[0]
        [10],[9],[8],[7],[6],[5],[4],[3],[2]
    [8],[7],[6],[5],[start],[3],[999],[1],[goal]
           [10],[9],[8],[7],[6],[5],[4],[3],[2]
          [11],[10],[9],[8],[7],[6],[5],[4],[3]
         [12],[11],[10],[9],[8],[7],[6],[5],[4]
        [13],[12],[11],[10],[9],[8],[7],[6],[5]
    */

    [System.Serializable]
    public class gridCollum {
        public List<int> items = new List<int>();
        
        public gridCollum(int size, int defaultVal) {
            items = new List<int>();
            for (int y = 0; y < size; y++) items.Add(defaultVal); 
        }
    }

    [System.Serializable]
    public class grid {
        [Header("size")]
        public int size = 50; // the size of the grid
        public float tileSize = 0.5f; // the width and length of a tile
        
        [Header("collisions")]
        public int layermask = 3; // hardcoded to the layer i use for the ground, id like to not do this but im too lazy to figure it out. Ill prolly come back to this
        
        [Header("path data")]
        public int maxPath = 50; // the max amount of steps a path can have, this is really just for like optimisation so it doesnt have to calculate the length of every path
        public int pathAttempts = 15; // how many times it will attempt to find the path
        
        [Header("accuracy")]
        public int distanceAccuracy = 100; // how accurate you will allow the distance to be
        public int acceptableDistance = 1; // how close you will allow it to be before it finishes

        [Header("dev")]
        public string status = "all good";

        // config stuff
        private Vector3 offset = new Vector3(0, 1, 0); // the offset of the rayhit on the ground just to like make sure it doesnt count the ground as an obsticle

        public List<gridCollum> tiles = new List<gridCollum>();
        // format: tiles[x][y]

        // simple constructor
        public grid() {this.generate();}

        public void generate() {
            this.tiles = new List<gridCollum>();
            for (int x = 0; x < size; x++) this.tiles.Add(new gridCollum(size, size * 999));
        }

        /*
            This is the main function really
                how this works in my plan is, you give it the self and the target
                it views your grid and makes note of the target position and your position (always dead center)
                it will go through every tile twice
                    first itteration: check if its blocked with a raycube, if not set distance to 0
                    second itteration: if the distance is 0 set it to the correct distance
                then itll go from the start point and pick the block with the closest distance to move to
                    if there are 2 of the same then pick a random one (probably not the best way of chosing but what do you expect from me)
                do this for a specified number of times and return the smallest succesfull path
                    if no path found then return null
        */
        public List<Vector3> findPath (Transform self, Transform target, int distanceLimi = 100) {
            status = "searching for path";
            // itterate through the grid to check if its a possible item
            for (int x = 0; x < this.tiles.Count - 1; x++) {
                for (int y = 0; y < this.tiles[x].items.Count - 1; y++) {
                    bool result = checkGrid(self, new Vector2((x - calculateOffsetForCo()) * this.tileSize, y - calculateOffsetForCo() * this.tileSize));
                    this.tiles[x].items[y] = result ? 0 : this.tiles[x].items[y];
                }
            }

            // itterate through the grid to calculate all the distances 
            bool foundSpace = false;
            for (int x = 0; x < this.tiles.Count - 1; x++) {
                for (int y = 0; y < this.tiles[x].items.Count - 1; y++) {
                    if (this.tiles[x].items[y] == 0) {
                        foundSpace = true;
                        Vector3 origin = self.position.Round() + new Vector3((x - calculateOffsetForCo()) * this.tileSize , 0, (y - calculateOffsetForCo()) * this.tileSize);
                        this.tiles[x].items[y] = (int)(Vector3.Distance(origin, target.position.Round()) * this.distanceAccuracy);
                    }
                }
            }

            if (!foundSpace) {
                status = "unable to find any spaces";
                return new List<Vector3>();
            }

            // begin path finding
            // make a list of all paths
            List<List<Vector3>> paths = new List<List<Vector3>>();
            for (int i = 0; i < this.pathAttempts - 1; i++) {
                List<Vector3> path = new List<Vector3>();
                
                int currentValue = 1000;
                int itteration = 0;

                bool finished = false;

                Vector2 currentPosition = new Vector2(calculateOffsetForCo(), calculateOffsetForCo());

                while (!finished && itteration <= this.maxPath) {
                    List<Vector2> possiblePositions = findSmallestPositions(currentPosition);
                    Vector2 nextPos = possiblePositions[UnityEngine.Random.Range(0, possiblePositions.Count -1)];

                    currentValue = this.tiles[(int)nextPos.x].items[(int)nextPos.y];
                    currentPosition = nextPos;

                    Vector3 WP = new Vector3();
                    path.Add(WP = self.position.Round() + new Vector3((nextPos.x - calculateOffsetForCo()) * this.tileSize, 0, (nextPos.y - calculateOffsetForCo()) * this.tileSize));
                    if (Vector3.Distance(WP, target.position) <= this.acceptableDistance) finished = true;

                    itteration++;
                }

                if (finished) paths.Add(path);
                else this.status = "unable to find a short enough path";
            }

            // find the smallest path
            if (paths.Count == 0) return new List<Vector3>();

            List<Vector3> smallestpath = paths[0];
            foreach (List<Vector3> item in paths) if (item.Count < smallestpath.Count) smallestpath = item;

            // return
            return smallestpath;
        }

        // a function to check if a cube has a 
        public bool checkGrid (Transform self, Vector2 gridPosition) {
            Vector3 origin = self.position.Round() + new Vector3(gridPosition.x, 0, gridPosition.y);

            // get the ground
            RaycastHit hit;
            if (Physics.Raycast(origin, -self.up, out hit, Mathf.Infinity)) {
                origin = hit.point.Round() + offset;
            } else return false;
            
            return !Physics.BoxCast(origin, new Vector3(this.tileSize / 2, this.tileSize / 2, this.tileSize / 2), new Vector3(0, 0, 0), Quaternion.identity, Mathf.Infinity, this.layermask);

            // return false;
        }

        // a util to check the direction with the smallest distance 
        public List<Vector2> findSmallestPositions(Vector2 current) {
            Dictionary<int, List<Vector2>> positions = new Dictionary<int, List<Vector2>>();

            foreach(Vector2 mod in new List<Vector2>{
                new Vector2(0, 1),
                new Vector2(0, -1),
                new Vector2(1, 0),
                new Vector2(-1, 0),
                new Vector2(-1, 1),
                new Vector2(1, 1),
                new Vector2(1, -1),
            }) {
                Vector2 working = current + mod;
                // Debug.Log($"data :: current: {current}, this.tiles.Count: {this.tiles.Count}, x: {working.x}, items.Count: {this.tiles[(int)working.x].items.Count}, y: {working.y}");

                if (!(working.x < 0 || working.y < 0 || working.x > this.size - 1 || working.y > this.size - 1)) {
                    int value = this.tiles[(int)working.x].items[(int)working.y];
                    if (positions.ContainsKey(value)) positions[value].Add(current + mod);
                    else positions.Add(value, new List<Vector2>{current + mod});
                }
            }

            return positions[positions.Keys.Min()];
        }

        public int calculateOffsetForCo() {return (int)(this.size / 2);}


    }
}