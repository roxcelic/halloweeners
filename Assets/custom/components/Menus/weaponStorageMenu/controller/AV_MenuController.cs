using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using ext;

using TMPro;

namespace AVdata {

    // a class to save an attack
    [System.Serializable]
    public class savedAttack {
        public string attackName = "";
        public attack.attackData attackData;

        public savedAttack(AT_base attack = null) {
            if (attack == null) {
                this.attackName = "";
                this.attackData = new attack.attackData();

                return;
            } // bleh

            this.attackName = attack.name;
            this.attackData = attack.attackData;
        }
    }

}

public class AV_MenuController : MonoBehaviour {
    [Header("prefabs")]
    public GameObject P_itemDisplay;

    [Header("components")]
    public Transform itemDisplay;
    public TMP_Text itemNameDisplay;
    public TMP_Text selectedItemNameDisplay;
    public TMP_Text pageDisplay;
    public TMP_Text statsDisplay;

    [Header("child menus")]
    public GameObject nameMenu;
    public TMP_InputField nameInput;

    public GameObject upgradeMenu;
    public TMP_Text UG_weaponName;
    public TMP_Text UG_weaponDamage;
    public TMP_Text UG_weaponLifeSteal;

    [Header("config")]
    public Vector3 itemStartLocation;
    public Vector2 itemOffset;
    public int itemsPerRow;

    public int itemsPerPage = 30;

    public List<string> barredAttackNames;

    [Header("data")]
    public int page = 0;
    public List<AVdata.savedAttack> currentAttacks;

    public AVdata.savedAttack selectedAttack = null;
    public int selectedIndex = 0;
    
    public bool inMenu = false;

    private bool selectedAnAttack = false;

    [Header("player")]
    public string playerTag = "Player";
    private playerController player;

    [Header("upgrade data")]
    public int baseCost = 100;
    public float upgradeAmount = 0.1f;
    public float upgradeMultiplier = 1.1f;

    [Header("text")]
    public sys.Text T_statsDisplay = new sys.Text();
    public sys.Text T_damage = new sys.Text();
    public sys.Text T_lifeSteal = new sys.Text();

    void Start() {player = GameObject.FindGameObjectsWithTag(playerTag)[0].transform.GetComponent<playerController>(); itemNameDisplay.text = "\"\"";}
    void OnEnable() {loadPages();}

    #region utils
    /// <summery> this function will load the current page</summery>
    public void loadPages() {
        // update the page display
        pageDisplay.text = $"{page + 1}/{findPageCount() + 1}";

        // destroy all current children
        foreach (Transform child in itemDisplay) Destroy(child.gameObject);

        // get the available attacks
        List<AVdata.savedAttack> savedAttacks = new List<AVdata.savedAttack>(save.getData.viewSave().savedAttacks);
        
        // calculate the items to show on the page
            int itemStartCutoff = (page) * itemsPerPage;
            int itemCutoff = Math.Clamp(savedAttacks.Count, 0, itemStartCutoff + itemsPerPage);


        savedAttacks = savedAttacks.Slice(itemStartCutoff, itemCutoff);

        // spawn gameObjects
        Vector3 currentSpawnLoc = itemStartLocation;
        int currentItemsPerRow = 0;
        for (int i = 0; i < savedAttacks.Count; i++) {
            // spawn
            GameObject item = Instantiate(P_itemDisplay, new Vector3(), Quaternion.identity, itemDisplay);
            item.transform.GetComponent<RectTransform>().anchoredPosition = currentSpawnLoc;

            // set the data
            AV_itemController itemController = item.transform.GetComponent<AV_itemController>();
            itemController.attack = savedAttacks[i];
            itemController.menu = this;
            itemController.index = itemStartCutoff + i;

            AT_base refrencedAttack = GS.live.state.getCurrentAttack(savedAttacks[i].attackName);

            if (refrencedAttack != null) itemController.image.sprite = refrencedAttack.sprite;
            else Debug.Log($"had issues finding the refrenced attack: {savedAttacks[i].attackName}");

            // update spawn position
            if (currentItemsPerRow >= itemsPerRow) {
                currentSpawnLoc = new Vector3(itemStartLocation.x, currentSpawnLoc.y - itemOffset.y, currentSpawnLoc.z);
                currentItemsPerRow = 0;
            } else {
                currentSpawnLoc = new Vector3(currentSpawnLoc.x + itemOffset.x, currentSpawnLoc.y, currentSpawnLoc.z);
                currentItemsPerRow++;
            }
        }
    }

    /// <summery> a function to update all the buttons with the selected attack </summery>
    public void updateAttack(AVdata.savedAttack attack, int index, bool SL = true) {
        // ability to reset the attack
        if (attack == null) {
            selectedItemNameDisplay.text = $"\"\"";

            selectedAttack = attack;
            selectedIndex = index;

            return;
        }

        selectedAttack = attack;
        selectedItemNameDisplay.text = $"\"{attack.attackName} :: {attack.attackData.name}\"";
        selectedIndex = index;

        selectedAnAttack = SL;
    }

    /// <summery> a function to get the last page </summery>
    public int findPageCount() {
        List<AVdata.savedAttack> savedAttacks = new List<AVdata.savedAttack>(save.getData.viewSave().savedAttacks);
        return (int)Mathf.Ceil(savedAttacks.Count / itemsPerPage);
    }

    /// <summery> a function to load the attacks stats </summery.
    public void loadStats (AVdata.savedAttack attack) {
        itemNameDisplay.text = $"{attack.attackName} :: {attack.attackData.name}";

        statsDisplay.text = T_statsDisplay.displayVar(new Dictionary<string, string>{
            {"type", attack.attackName},
            {"name", attack.attackData.name},
            {"killCount", attack.attackData.killCount.ToString()},
            {"damageModifier", attack.attackData.damageModifier.ToString()},
            {"lifeStealModifer", attack.attackData.lifeStealModifer.ToString()},
        });
    }

    /// <summery> a function to open the attack name edit </summery>
    public void openName(bool open = true) {
        if (upgradeMenu.activeSelf) return;

        inMenu = open;
        if (!selectedAnAttack || selectedAttack == null) return;
        if (open) {
            nameInput.text = selectedAttack.attackData.name;
        }

        nameMenu.SetActive(open);
    }
    
    /// <summery> a function to edit the name of an attack </summery>
    public void saveName() {
        selectedAttack.attackData.name = nameInput.text;
        
        save.saveData currentSave = save.getData.viewSave();
        currentSave.savedAttacks[selectedIndex] = selectedAttack;
        save.getData.save(currentSave);

        loadPages(); // reload the page
    }

    /// <summery> a function to open the attakc upgrade </summery>
    public void openUpgrade(bool open = true) {
        if (nameMenu.activeSelf) return;
        inMenu = open;
        if (!selectedAnAttack || selectedAttack == null) return;
        if (open) {
            UG_weaponName.text = $"\"{selectedAttack.attackName} :: {selectedAttack.attackData.name} :: {selectedAttack.attackData.killCount}\"";
            UG_weaponDamage.text = $"{T_damage.localise()}: {selectedAttack.attackData.damageModifier} :: {costOfUpgrade(selectedAttack.attackData.damageModifier, 1)}";
            UG_weaponLifeSteal.text = $"{T_lifeSteal.localise()}: {selectedAttack.attackData.lifeStealModifer} :: {costOfUpgrade(selectedAttack.attackData.lifeStealModifer)}";
        }

        upgradeMenu.SetActive(open);
    }
    
    /// <summery> a function to upgrade an attacks life steal </summery>
    public void upgradeLifeSteal() {
        if (selectedAttack.attackData.killCount < costOfUpgrade(selectedAttack.attackData.lifeStealModifer)) return;
        selectedAttack.attackData.killCount -= costOfUpgrade(selectedAttack.attackData.lifeStealModifer);
        selectedAttack.attackData.lifeStealModifer += upgradeAmount;

        save.saveData currentSave = save.getData.viewSave();
        currentSave.savedAttacks[selectedIndex] = selectedAttack;
        save.getData.save(currentSave);

        loadPages(); // reload the page
        openUpgrade(); // reload the upgrade page
    }

    /// <summery> a function to upgrade an attacks damage</summery>
    public void upgradeDamage() {
        if (selectedAttack.attackData.killCount < costOfUpgrade(selectedAttack.attackData.damageModifier, 1)) return;
        selectedAttack.attackData.killCount -= costOfUpgrade(selectedAttack.attackData.damageModifier, 1);
        selectedAttack.attackData.damageModifier += upgradeAmount;

        save.saveData currentSave = save.getData.viewSave();
        currentSave.savedAttacks[selectedIndex] = selectedAttack;
        save.getData.save(currentSave);

        loadPages(); // reload the page
        openUpgrade(); // reload the upgrade page
    }

    /// <summery> a function to calculate the cost of an upgrade </summery>
    public int costOfUpgrade(float current, int modifier = 0) {
        return baseCost * (int)(((current - modifier) / upgradeAmount) * upgradeMultiplier);
    }

    /// <summery> a general close menu button </summery>
    public void closeMenu() {
        if (upgradeMenu.activeSelf) upgradeMenu.SetActive(false);
        else if (nameMenu.activeSelf) nameMenu.SetActive(false);

        inMenu = false;
    }

    #endregion

    #region buttons utils
    /// <summery> this function will move the page by -1 and load the page </summery>
    public void pageLeft() {
        Debug.Log("left");
        page--;
        if (page < 0) page = findPageCount();

        loadPages();
    }

    /// <summery> this function will move the page by +1 and load the page </summery>
    public void pageRight() {
        Debug.Log("right");
        page++;
        if (page > findPageCount()) page = 0;

        loadPages();
    }

    /// <summery> this function is a developer function to add an item to the saved data </summery>
    public void addItemToSavedData() {
        save.saveData currentSave = save.getData.viewSave();
        currentSave.savedAttacks.Add(new AVdata.savedAttack());
        save.getData.save(currentSave);
    }

    /// <summery> the **actual** function to save an attack </summery>
    public void saveHeldAttack() {
        AT_base cachedAttack = player.attack;

        // if a default attack dont continue
        if (barredAttackNames.Contains(cachedAttack.name)) return;
        player.switchAttack(player.D_Attack); // set the players current attack to the base attack
        
        // save it
        save.saveData currentSave = save.getData.viewSave();
        currentSave.savedAttacks.Add(new AVdata.savedAttack(cachedAttack));
        save.getData.save(currentSave);

        loadPages(); // reload the page
    }

    /// <summery> the function to give the attack to the player </summery>
    public void loadSavedAttack() {
        if (!selectedAnAttack || selectedAttack == null || inMenu) return;

        AT_base cachedAttack = player.attack;

        AT_base loadedAttack = Instantiate(GS.live.state.getCurrentAttack(selectedAttack.attackName));
        loadedAttack.attackData = selectedAttack.attackData;

        player.switchAttack(loadedAttack);
        save.saveData currentSave = save.getData.viewSave();

        // if a default attack dont swap
        if (barredAttackNames.Contains(cachedAttack.name)) {
            currentSave.savedAttacks.RemoveAt(selectedIndex);
            save.getData.save(currentSave);            

            loadPages();
            updateAttack(null, 0, false);

            return;
        }

        currentSave.savedAttacks[selectedIndex] = new AVdata.savedAttack(cachedAttack);
        save.getData.save(currentSave);

        loadPages(); // reload the page

        // unselect
        updateAttack(null, 0, false);
    }

    /// <summery> a function to delete the saved attack </summery>
    public void deleteSavedAttack() {
        if (!selectedAnAttack || inMenu) return;

        save.saveData currentSave = save.getData.viewSave();
        if (currentSave.savedAttacks.Count >= selectedIndex) currentSave.savedAttacks.RemoveAt(selectedIndex);
        save.getData.save(currentSave);   

        updateAttack(null, 0, false);

        loadPages();
    }
    #endregion


}
