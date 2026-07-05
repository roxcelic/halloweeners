using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class L1_Puzzle : MonoBehaviour {
    public static L1_Puzzle self;
    public List<level1.puzzleEntry> patterns = new List<level1.puzzleEntry>();
    public List<int> history = new List<int>();

    void Start() {self = this;}

    public static void RunUpdate(int index) {
        self.history.Add(index);

        foreach (level1.puzzleEntry ent in self.patterns) {
            if (ent.match(self.history)){
                sys.utils.displayOnPlayer(new sys.Text(sys.text.displayKeyButton(ent.rewardText.localise())));
                ent.complete();
                foreach(sys.entityPositionRef thing in ent.enetities){
                    thing.spawn();
                }
            }
        }
    }
}