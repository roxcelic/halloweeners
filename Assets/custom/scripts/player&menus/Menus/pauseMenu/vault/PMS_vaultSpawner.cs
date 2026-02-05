using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using save;

using player.utils;

public class PMS_vaultSpawner : displayVarItems {
    public static PMS_vaultSpawner instance;

    [Header("pause menu vault")]
    public int page = 0;
    public int itemsPerPage => horizontal * verticle;

    [Header("comp")]
    public GameObject selectionMenu;
    public TMP_Text selectionDisplay;
    public TMP_Text selectionDisplayDescription;

    [Header("text")]
    public sys.Text message = new sys.Text();

    [Header("data")]
    public int selectedItem;
    private AT_base currentSelectedAttack => GS.live.state.getCurrentAttack(getData.viewSave().savedAttacks[selectedItem].attackName);

    void Start() {
        instance = this;
    }

    public override GameObject editSpawn(GameObject spawned, Vector2 set) {
        PMS_vaultItem item = spawned.transform.GetComponent<PMS_vaultItem>();
        item.position = (int)(((set.x + 1) + set.y * horizontal) + (itemsPerPage * page)) - 1;
        item.display();

        return spawned;
    }

    protected override void OnDisable() {
        openChildMenu(true);
        base.OnDisable();
    }

    /// <summery> moves the page right or resets if no more pages </summery>
    public void pageRight() {
        if (spawning) return;

        saveData currentSave = getData.viewSave();
        int pageCount = (int)Mathf.Ceil(currentSave.savedAttacks.Count / itemsPerPage);

        page++;
        if (page > pageCount) page = 0;

        reset();
    }

    /// <summery> moves the page left or to the furthest right if 0 </summery>
    public void pageLeft() {
        if (spawning) return;

        page--;
        if (page < 0) {
            saveData currentSave = getData.viewSave();
            page = (int)Mathf.Ceil(currentSave.savedAttacks.Count / itemsPerPage);
        }
        reset();
    }

    /// <summery> deposit attack </summery>
    public void depositAttack() {
        if (spawning) return;

        saveData currentSave = getData.viewSave();
        AT_base attack = playerController.mainPlayer.Reset();
        if (attack != null) {
            currentSave.savedAttacks.Add(new AVdata.savedAttack(attack));
            getData.save(currentSave);

            reset();
        }
    }


    /// <summery> a util to get the sprite of an attack </summery>
    public static Sprite? findAttackSprite(int index) {
        saveData currentSave = getData.viewSave();
        if (index >= currentSave.savedAttacks.Count) return null;
        else return GS.live.state.getCurrentAttack(currentSave.savedAttacks[index].attackName).sprite;
    }

    /// <summery> opens or closes the child menu </summery>
    public void openChildMenu(bool state = true) {
        selectionMenu.SetActive(!state);
        saveData currentSave = getData.viewSave();

        if(selectedItem >= currentSave.savedAttacks.Count) return;

        selectionMenu.SetActive(state);
        if (state) {

            AT_base selectedAttack = currentSelectedAttack;

            selectionDisplay.text = message.displayVar(new Dictionary<string, string> {
                {"item", selectedAttack.name}
            });

            selectionDisplayDescription.text = selectedAttack.description.localise();
        }
    }

    /// <summery> equips the selected weapon </summery>
    public void equipSelectedWeapon() {
        if (spawning) return;

        saveData currentSave = getData.viewSave();

        if(selectedItem >= currentSave.savedAttacks.Count) return;
        AT_base attack = playerController.mainPlayer.Reset();

        playerController.mainPlayer.switchAttack(currentSelectedAttack);

        if (attack != null) currentSave.savedAttacks[selectedItem] = new AVdata.savedAttack(attack);
        else currentSave.savedAttacks.RemoveAt(selectedItem);

        getData.save(currentSave);

        reset();
    }

}