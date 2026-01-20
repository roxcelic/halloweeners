using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

public class PMS_vaultControls : MonoBehaviour {
    /// <summery> moves the pause menu vault to the right </summery>
    public void right() {PMS_vaultSpawner.instance.pageRight();}

    /// <summery> moves the pause menu vault to the left </summery>
    public void left() {PMS_vaultSpawner.instance.pageLeft();}

    /// <summery> deposit attack </summery>
    public void depositAttack() {PMS_vaultSpawner.instance.depositAttack();}

    /// <summery> closes the sub menu </summery>
    public void closeSubMenu() {PMS_vaultSpawner.instance.openChildMenu(false);}

    /// <summery> equip the selected attack </summery>
    public void equip() {PMS_vaultSpawner.instance.equipSelectedWeapon();}
}