using UnityEngine;

public class CombatEvent : MonoBehaviour
{
    public void StartCombat()
    {
        CombatEntities _combatEntities = CombatEntities.instance;

        _combatEntities.player = PlayerController.instance.gameObject;
        _combatEntities.enemy = gameObject;

        gameObject.AddComponent<EnemySpawner>();

        CombatController.instance.StartCombat();
    }
}
