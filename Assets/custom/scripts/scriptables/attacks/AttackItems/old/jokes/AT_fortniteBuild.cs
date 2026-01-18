using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

[CreateAssetMenu(fileName = "new attack", menuName = "attacks/jokes/fortnite")]
public class AT_fortniteBuild : AT_base {
    public GameObject pointer = null;
    public GameObject ui = null;
    [Range(0, 10)] public int gridSize = 10;
    private string selected;
    private Vector3 pointerOffset;
    private Vector3 rotOffset;
    private Vector3 rot;

    [Header("globals")]
    public static string fortniteTag = "Fortnite";

    public override void load(playerController character, bool reload = true) {
        character.AttackDisplay.runtimeAnimatorController = AC;

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
        
        if (pointer == null) changeActive("1");
        pointer.transform.position = pos + pointerOffset;
        Vector3 tmpRot = rot + rotOffset;
        pointer.transform.rotation = Quaternion.Euler(tmpRot.x, tmpRot.y, tmpRot.z);

        if (Input.GetKeyDown("z")) {changeActive("1");}
        if (Input.GetKeyDown("x")) {changeActive("2");}
        if (Input.GetKeyDown("c")) {changeActive("3");}
        if (Input.GetKeyDown("v")) {changeActive("4");}

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {rot += new Vector3(0, 90, 0);}
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {rot -= new Vector3(0, 90, 0);}
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
        block.transform.position += pos;

        Vector3 tmpRot = rot + rotOffset;
        block.transform.rotation = Quaternion.Euler(tmpRot.x, tmpRot.y, tmpRot.z);
        block.layer = 3;
        block.tag = fortniteTag;
    }


    #region util
    public void changeActive(string newActive) {
        selected = newActive;
        if(pointer != null) Destroy(pointer);
        pointer = Instantiate(Resources.Load<GameObject>($"weapons/dev/fortnite/{newActive}"));
        pointerOffset = pointer.transform.position;
        rotOffset = pointer.transform.rotation.eulerAngles;
        pointer.transform.GetComponent<Collider>().isTrigger = true;
        ui.transform.GetComponent<Animator>().Play(newActive);
    }
    #endregion

}