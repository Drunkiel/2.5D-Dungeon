using System.Collections;
using UnityEngine;

public class EntityCombat : MonoBehaviour
{
    public bool canCombat = true;
    public bool inCombat;
    public float timeToResetCombat;

    public void ManageCombat(Transform transform)
    {
        if (!canCombat)
            return;

        timeToResetCombat = 0;

        if (!inCombat)
        {
            StartCoroutine(ResetCombat());
            if (TryGetComponent(out EntityController _entityController))
            {
                _entityController.currentState = State.Attacking;
                _entityController._entityWalk.targetTransform = transform;
            }

            inCombat = true;
        }
    }

    private IEnumerator ResetCombat()
    {
        //Wait to reset
        while (timeToResetCombat < 5)
        {
            timeToResetCombat += 1;
            yield return new WaitForSeconds(1f);
        }

        if (TryGetComponent(out EntityController _entityController))
            _entityController.currentState = State.Patroling;

        inCombat = false;
    }
}
