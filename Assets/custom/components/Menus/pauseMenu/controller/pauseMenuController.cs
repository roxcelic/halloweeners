using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

public class pauseMenuController : MonoBehaviour {
    [Header("comp")]
    public GameObject PM_Item_Prefab;
    public Transform pauseMenu;

    [Header("items")]
    public List<MM_base> currentItems;
    public List<List<MM_base>> previousItems = new List<List<MM_base>>();
    public List<pauseMenuItem> spawnedItems;

    [Header("data")]
    public int selected;

    public float Ydisplacement = 50f;
    [Range(0, 25f)] public float xMargin = 10f;

    void Start() {
        
    }

    void Update() {
        int selectionOffset = 0;

        if (eevee.input.Grab("Pause", "pm")) changePauseState(!GS.live.state.paused);
        if (!GS.live.state.paused) return;

        // up
        if (eevee.input.Collect("up", "pm")) selectionOffset--;

        // down
        if (eevee.input.Collect("down", "pm")) selectionOffset++;

        changeSelected(selectionOffset);

        // interact
        if (eevee.input.Collect("interact", "pm")) currentItems[selected].action(null, this);

        // back
        if (eevee.input.Collect("back", "pm")) loadPrevMenu();
    }

    public void loadMenu() {
        unloadMenu();

        Vector3 spawnPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        foreach (MM_base item in currentItems) {
            spawnMenuItem(new Vector3 (UnityEngine.Random.Range(xMargin, Screen.width - xMargin), spawnPos.y, spawnPos.z), item);
            spawnPos += new Vector3(0, -Ydisplacement, 0);
        }
    }

    public void unloadMenu() {
        foreach (Transform child in pauseMenu) {
            Destroy(child.gameObject);
        }
    }

    /* 
        utils and such
    */
    #region utils
    // a util to assist with changing the pause state to turn the menu on and off 
    public void changePauseState(bool newVal) {
        GS.live.state.pause(newVal);
        transform.GetChild(0).gameObject.SetActive(newVal);
        
        if (newVal) loadMenu();
        else unloadMenu();
    }
    // a util to change the selected item
    public void changeSelected(int offset, bool force = false) {
        if (!force && offset == 0) return;
        // spawnedItems[selected].selected = false;
        
        if (force) selected = offset;
        else selected += offset;

        if (selected < 0) selected = currentItems.Count - 1; // if too low select the bottom
        else if (selected > currentItems.Count - 1) selected = 0; // if too high select the top
    
        // spawnedItems[selected].selected = true;
    }

    // a util to load the next menu
    public void loadNextMenu(List<MM_base> menuItems) {
        if (menuItems.Count == 0) return;

        previousItems.Add(currentItems);
        currentItems = menuItems;
        changeSelected(0, true);

        loadMenu();
    }

    // a util to load the previous menu
    public void loadPrevMenu() {
        currentItems = previousItems[previousItems.Count - 1];
        changeSelected(0, true);
    
        loadMenu();
    }

    // a util to spawn a menu
    public GameObject spawnMenuItem(Vector3 pos, MM_base item) {
        GameObject tmpItem = Instantiate(PM_Item_Prefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity, pauseMenu);
        pauseMenuItem tmpPMI = tmpItem.transform.GetComponent<pauseMenuItem>();
        
        tmpPMI.PMC = this;
        tmpPMI.text.text = item.name;
        tmpPMI.menuItem = item;

        return tmpItem;
    }

    // spawn a menu Item
    #endregion
}