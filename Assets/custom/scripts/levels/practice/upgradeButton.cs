using UnityEngine;

public class updgradeButton : MonoBehaviour {
    public stats.config.upgradeTypes upgrade;
    public upgradeScreen screen;

    public void Press() {screen.Submit(upgrade);}
}