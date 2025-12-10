using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using ext;

/*
    This script refrences the textures as a material a lot and thats because i planned it to be a material based system
    but later changed it
*/
[CreateAssetMenu(fileName = "new attack", menuName = "attacks/jokes/minecraft")]
public class AT_minecraftCreativeMode : AT_base {
    private int index = 0;

    [Header("minecraft config")]
    public GameObject pointer = null;
    [Range(0, 10)] public int gridSize = 10;
    public List<sys.texWithName> materials;

    [Header("globals")]
    public static string minecraftTag = "Minecraft";

    public override void load(playerController character, bool reload = true) {
        character.AttackDisplay.runtimeAnimatorController = AC;
        character.crosshairDisplay.runtimeAnimatorController = crosshair;

        pointer = Instantiate(Resources.Load<GameObject>("weapons/general/cube"));
        pointer.transform.GetComponent<Collider>().isTrigger = true;
        updateMat(0);
    }

    public override void unLoad(playerController character) {
        Destroy(pointer);
    }

    public async override void update(playerController character) {
        Vector3 pos = getAimedLocation(character.transform, character);
        Vector3 distance = character.transform.position - pos;

        pos = new Vector3(
            distance.x > 0 ? pos.x.roundToNearestCeil(gridSize) : pos.x.roundToNearest(gridSize), 
            distance.y > 0 ? pos.y.roundToNearestCeil(gridSize) : pos.y.roundToNearest(gridSize), 
            distance.z > 0 ? pos.z.roundToNearestCeil(gridSize) : pos.z.roundToNearest(gridSize)
        );

        pointer.transform.position = pos;

        if (Input.GetMouseButtonDown(1) && !GS.live.state.paused && !GS.live.state.menued && !GS.live.state.helped) {
            List<string> matNames = new List<string>();
            foreach(sys.texWithName mat in materials) matNames.Add(mat.name);
            updateMat(await openMenu(matNames, index));
        }
    } 

    public override void attack(playerController character) {
        Vector3 pos = getAimedLocation(character.transform, character);
        Vector3 distance = character.transform.position - pos;

        pos = new Vector3(
            distance.x > 0 ? pos.x.roundToNearestCeil(gridSize) : pos.x.roundToNearest(gridSize), 
            distance.y > 0 ? pos.y.roundToNearestCeil(gridSize) : pos.y.roundToNearest(gridSize), 
            distance.z > 0 ? pos.z.roundToNearestCeil(gridSize) : pos.z.roundToNearest(gridSize)
        );

        GameObject block = Instantiate(Resources.Load<GameObject>("weapons/general/cube"));
        block.transform.position += pos;

        block.layer = 3;
        block.tag = minecraftTag;
        block.GetComponent<Renderer>().material.SetTexture("_MainTex", materials[index].tex);
    }

    public void updateMat(int newIndex) {
        index = newIndex;

        pointer.GetComponent<Renderer>().material.SetTexture("_MainTex", materials[newIndex].tex);
    }
}