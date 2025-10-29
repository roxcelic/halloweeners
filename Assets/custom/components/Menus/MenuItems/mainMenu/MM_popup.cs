using UnityEngine;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/popup")]
public class MM_popup : MM_base {
    public GameObject popup;
    public Sprite popupDisplay;
    public string description;

    public override void action(mainMenuController MMC = null, pauseMenuController PMC = null) {
        MMC.canMove = false;

        GameObject spawned = GameObject.Instantiate(popup);

        // positioning
        spawned.transform.GetComponent<RectTransform>().SetParent(MMC.transform);
        spawned.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3();

        // dynamic close
        spawned.transform.GetComponent<uiuitels>().dynamicCode = () => {MMC.canMove = true;};

        // idek
        popup pops = spawned.transform.GetComponent<popup>();
        pops.MMC = MMC;
        pops.onComplete = () => {Debug.Log("forward");};
        pops.onDeny = () => {
            spawned.transform.GetComponent<Animator>().Play("close");
        };

        // display
        pops.display.sprite = popupDisplay;
        pops.description.text = description;
        
    }
}
