using UnityEngine;

public class POR_Puzzle : POR_BASE {
    public int index = 0;

    protected override void OnTp() {
        sys.utils.displayOnPlayer(new sys.Text(sys.text.displayKeyButton($"{index.ToString()}")));
        L1_Puzzle.RunUpdate(index);
    }
}