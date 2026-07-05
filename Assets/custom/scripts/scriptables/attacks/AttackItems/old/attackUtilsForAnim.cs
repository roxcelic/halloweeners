using UnityEngine;

using player.utils;

public class attackUtilsForAnim : MonoBehaviour {
    public playerController player;

    void Start() {if (player == null) player = playerController.mainPlayer;}

    public void attack() {player.extraAttack();}
    public void ability() {player.runAbility();}
    public void reload() {player.attack.reload();}
}