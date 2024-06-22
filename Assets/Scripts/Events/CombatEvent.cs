using UnityEngine;

public class CombatEvent : MonoBehaviour
{
    public void StartCombat()
    {
        CombatEntities.instance.player = PlayerController.instance.gameObject;
        CombatEntities.instance.enemy = gameObject;

        CombatController.instance.StartCombat();
    }
}
