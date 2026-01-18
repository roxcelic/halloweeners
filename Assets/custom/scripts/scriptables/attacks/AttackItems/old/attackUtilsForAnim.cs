using UnityEngine;

public class attackUtilsForAnim : MonoBehaviour {
    public playerController player;

    public void attack() {player.extraAttack();}
    public void ability() {player.runAbility();}
}