using UnityEngine;

public class CombatEvent : MonoBehaviour
{
    public void StartCombat()
    {
        CombatEntities _combatEntities = CombatEntities.instance;

        _combatEntities.player = PlayerController.instance.gameObject;
        _combatEntities.enemy = gameObject;

        CombatController.instance.StartCombat();
    }
}
