using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/fortnite")]
public class AT_fortniteBuild : AT_base {
    public GameObject pointer = null;
    public GameObject ui = null;
    [Range(0, 10)] public int gridSize = 10;
    private string selected;

    public override void load(playerController character, bool reload = true) {
        character.AttackDisplay.runtimeAnimatorController = AC;
        character.crosshairDisplay.runtimeAnimatorController = crosshair;

        sys.utils.log("legally distinct build mechanics");
        sys.utils.displayOnPlayer(new sys.Text("legally distinct build mechanics activate!"));

        ui = Instantiate(Resources.Load<GameObject>("weapons/dev/fortniteUI"));
        changeActive("1");
    }

    public override void unLoad(playerController character) {
        Destroy(ui);
        Destroy(pointer);
    }

    public override void update(playerController character) {
        Vector3 pos = getAimedLocation(character.transform, character);
        Vector3 distance = character.transform.position - pos;

        pos = new Vector3(
            distance.x > 0 ? pos.x.roundToNearestCeil(gridSize) : pos.x.roundToNearest(gridSize), 
            distance.y > 0 ? pos.y.roundToNearestCeil(gridSize) : pos.y.roundToNearest(gridSize), 
            distance.z > 0 ? pos.z.roundToNearestCeil(gridSize) : pos.z.roundToNearest(gridSize)
        );
        
        pointer.transform.position = pos;

        if (Input.GetKeyDown("z")) {changeActive("1");}
        if (Input.GetKeyDown("x")) {changeActive("2");}
        if (Input.GetKeyDown("c")) {changeActive("3");}
        if (Input.GetKeyDown("v")) {changeActive("4");}
    } 

    public override void attack(playerController character) {
        Vector3 pos = getAimedLocation(character.transform, character);
        Vector3 distance = character.transform.position - pos;

        pos = new Vector3(
            distance.x > 0 ? pos.x.roundToNearestCeil(gridSize) : pos.x.roundToNearest(gridSize), 
            distance.y > 0 ? pos.y.roundToNearestCeil(gridSize) : pos.y.roundToNearest(gridSize), 
            distance.z > 0 ? pos.z.roundToNearestCeil(gridSize) : pos.z.roundToNearest(gridSize)
        );

        GameObject block = Instantiate(Resources.Load<GameObject>($"weapons/dev/fortnite/{selected}"));
        block.transform.position = pos;
        block.layer = 3;
    }


    #region util
    public void changeActive(string newActive) {
        selected = newActive;
        if(pointer != null) Destroy(pointer);
        pointer = Instantiate(Resources.Load<GameObject>($"weapons/dev/fortnite/{newActive}"));
        pointer.transform.GetComponent<Collider>().isTrigger = true;
        ui.transform.GetComponent<Animator>().Play(newActive);
    }
    #endregion

}