using UnityEngine;

public class CombatEvent : MonoBehaviour
{
    public void StartCombat()
    {
        CombatController.instance.StartCombat();
    }
}
