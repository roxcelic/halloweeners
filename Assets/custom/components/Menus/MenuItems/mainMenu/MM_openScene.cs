using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/changeScene")]
public class MM_changeScene : MM_base {
    public GameObject popup;
    public Sprite popupDisplay;
    public sys.Text description = new sys.Text();
    public string newScene;

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
        pops.onComplete = () => {
            SceneManager.LoadScene(newScene);
        };
        pops.onDeny = () => {
            spawned.transform.GetComponent<Animator>().Play("close");
        };

        // display
        pops.display.sprite = popupDisplay;
        pops.description.text = description.localise();
        
    }
}
