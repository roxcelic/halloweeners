using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace roomData {
    public enum roomType {
        single
    }
    public enum roomDirections {
        up,
        down,
        left,
        right
    }

    [System.Serializable]
    public class map {
        public int width;
        public int height;
        
        public mapItem[] mapItems;

        public map(int gridHeight, int gridWidth, room StartRoom) {
            height = gridHeight;
            width = gridWidth;

            mapItems = new mapItem[height];

            for (int i = 0; i < mapItems.Length; i++) {
                mapItems[i] = new mapItem(width);
            }

            mapItems[height / 2].rooms[width / 2] = StartRoom;
        }

        public bool play (List<room> possibleRooms, room emptyRoom) {
            bool changed = false;

            for (int i = 0; i < mapItems.Length; i++) {
                for (int z = 0; z < mapItems[i].rooms.Length; z++) {
                    if (mapItems[i].rooms[z] == null) {
                        List<room> foundRooms = getPossibleRooms(new Vector2(z,i), possibleRooms, emptyRoom);
                        
                        if (foundRooms.Count > 0) {
                            string rooms = "";
                            foreach (room room in foundRooms) rooms += room;

                            mapItems[i].rooms[z] = foundRooms[UnityEngine.Random.Range(0, foundRooms.Count)];

                            changed = true;
                        }
                    }
                }
            }

            return changed;
        }

        public List<room> filterRooms(List<room> possibleRooms, roomDirections direction, bool cantHave = true) {
            List<room> newRooms = new List<room>();

            foreach (room room in possibleRooms) {
                if(cantHave) {
                    if (!room.allowedDirections.Contains(direction)) newRooms.Add(room);
                } else {
                    if (room.allowedDirections.Contains(direction)) newRooms.Add(room);
                }
            }

            return newRooms;
        }

        public List<room> getPossibleRooms(Vector2 position, List<room> possibleRooms, room emptyRoom) {
            // if (!checkForNeighbors(position, emptyRoom)) return new List<room>();
            List<room> allowedRooms = new List<room>(possibleRooms);

            // directions it has to contain
            List<roomDirections> mandatoryDirections = getMandatoryDirections(position);
            if (mandatoryDirections.Count == 0) return new List<room>(); // return if no mandatory directions
            foreach (roomDirections dir in mandatoryDirections) {
                allowedRooms = filterRooms(allowedRooms, dir, false);
            }

            // directions it cannot contain
            List<roomDirections> impossibleDirections = getImpossibleDirections(position, emptyRoom);
            foreach (roomDirections dir in impossibleDirections) {
                allowedRooms = filterRooms(allowedRooms, dir);
            }

            return allowedRooms;
        }

        // function to check which directions it must be placed at
        public List<roomDirections> getMandatoryDirections(Vector2 position) {
            List<roomDirections> mandatoryDirections = new List<roomDirections>();
            room tmpRoom = null;

            // vertical
            if (position.y != 0) {
                tmpRoom = getRoom(position + new Vector2(0, -1));

                if (tmpRoom != null && tmpRoom.allowedDirections.Contains(roomDirections.down)) mandatoryDirections.Add(roomDirections.up);
            } // up

            if (position.y < height - 1) {
                tmpRoom = getRoom(position + new Vector2(0, 1));

                if (tmpRoom != null && tmpRoom.allowedDirections.Contains(roomDirections.up)) mandatoryDirections.Add(roomDirections.down);
            } // down
            
            // horizontal

            if (position.x < width - 1) {
                tmpRoom = getRoom(position + new Vector2(1, 0));

                if (tmpRoom != null && tmpRoom.allowedDirections.Contains(roomDirections.left)) mandatoryDirections.Add(roomDirections.right);
            } // right

            if (position.x != 0) {
                tmpRoom = getRoom(position + new Vector2(-1, 0));

                if (tmpRoom != null && tmpRoom.allowedDirections.Contains(roomDirections.right)) mandatoryDirections.Add(roomDirections.left);
            } // left
        
            return mandatoryDirections;
        }

        /* function to check which directions it cannot be placed at
            what conditions do i have to check to see if it cant point that way hmmmm   
                if the block in that direction is an end block
                if there is a block there and it doesnt have a point to connect
                if there is a block and its equal to the empty room
        */
        public List<roomDirections> getImpossibleDirections(Vector2 position, room emptyRoom) {
            List<roomDirections> impossibleDirections = new List<roomDirections>();
            room tmpRoom = null;

            // vertical

            if (position.y == 0) impossibleDirections.Add(roomDirections.up);
            else {
                tmpRoom = getRoom(position + new Vector2(0, -1));
                
                if (tmpRoom == emptyRoom) impossibleDirections.Add(roomDirections.up);
                else if (tmpRoom != null && !tmpRoom.allowedDirections.Contains(roomDirections.down)) impossibleDirections.Add(roomDirections.up);
            } // up

            if (position.y == height - 1) impossibleDirections.Add(roomDirections.down);
            else {
                tmpRoom = getRoom(position + new Vector2(0, 1));
                
                if (tmpRoom == emptyRoom) impossibleDirections.Add(roomDirections.down);
                else if (tmpRoom != null && !tmpRoom.allowedDirections.Contains(roomDirections.up)) impossibleDirections.Add(roomDirections.down);
            } // down

            // horizontal

            if (position.x == width - 1) impossibleDirections.Add(roomDirections.right);
            else {
                tmpRoom = getRoom(position + new Vector2(1, 0));
                
                if (tmpRoom == emptyRoom) impossibleDirections.Add(roomDirections.right);
                else if (tmpRoom != null && !tmpRoom.allowedDirections.Contains(roomDirections.left)) impossibleDirections.Add(roomDirections.right);
            } // right

            if (position.x == 0) impossibleDirections.Add(roomDirections.left);
            else {
                tmpRoom = getRoom(position + new Vector2(-1, 0));
                
                if (tmpRoom == emptyRoom) impossibleDirections.Add(roomDirections.left);
                else if (tmpRoom != null && !tmpRoom.allowedDirections.Contains(roomDirections.right)) impossibleDirections.Add(roomDirections.left);
            } // left

            return impossibleDirections;
        }

        // utility funtion to get the current room
        public room getRoom(Vector2 position) {
            return mapItems[(int)position.y].rooms[(int)position.x];
        }
    }

    [System.Serializable]
    public class mapItem {
        public room[] rooms;

        public mapItem(int arraySize) {
            rooms = new room[arraySize];
        }
    }
}