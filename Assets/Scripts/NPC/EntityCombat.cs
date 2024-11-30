using System.Collections;
using UnityEngine;

public class EntityCombat : MonoBehaviour
{
    public bool inCombat;
    public float timeToResetCombat;

    public void ManageCombat()
    {
        timeToResetCombat = 0;

        if (!inCombat)
        {
            StartCoroutine(ResetCombat());
            if (TryGetComponent(out EnemyController _enemyController)) 
                _enemyController.currentState = State.Attacking;

            inCombat = true;
        }
    }

    private IEnumerator ResetCombat()
    {
        //Wait 5s then set inCombat to false
        while (timeToResetCombat < 5)
        {
            timeToResetCombat += 1;
            yield return new WaitForSeconds(1f);
        }

        if (TryGetComponent(out EnemyController _enemyController))
            _enemyController.currentState = State.Patroling;

        inCombat = false;
    }
}
