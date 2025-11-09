using UnityEngine;
using UnityEngine.UI;

public class AV_itemController : MonoBehaviour {
    [Header("components")]
    public Image image;
    public AVdata.savedAttack attack;
    public AV_MenuController menu;
    public int index;

    #region utils
    /// <summery> a util for the on press </summery>
    public void onPress() {menu.updateAttack(attack, index);}

    /// <summery> a util to load the hovered data </summery>
    public void onHover() {menu.loadStats(attack);}
    #endregion
}