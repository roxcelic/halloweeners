using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace level1 {
    [Serializable]
    public class puzzleEntry {
        public List<int> path = new List<int>();
        public sys.Text rewardText = new sys.Text();
        public List<sys.entityPositionRef> enetities = new List<sys.entityPositionRef>();
        private bool ran = false;

        public void complete() {
            ran = true;
        }

        public bool match(List<int> fullPath) {
            if (ran) return false;

            int startIndex = fullPath.Count - path.Count;
            if (startIndex < 0) return false;

            List<int> pathToMatch = fullPath.Skip(startIndex).ToList();

            bool gotItRight = true;
            for (int number = 0; number < pathToMatch.Count; number++) {
                if (gotItRight) gotItRight = pathToMatch[number] == path[number];
            }

            return gotItRight;
        }
    }
}