using System;
using System.Collections;
using UnityEngine;

public class TeleportEvent : MonoBehaviour
{
    [SerializeField] private bool isOnCooldown;
    public Vector3[] positions;
    [SerializeField] private Animator anim;

    public void TeleportToPosition(int positionIndex)
    {
        if (isOnCooldown || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
            return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            PortalController.instance.TeleportToPosition(positions[positionIndex]);
            SetCooldown();
        }));
    }

    public void TeleportToObject(Transform objectTransform)
    {
        if (isOnCooldown || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
            return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            PortalController.instance.TeleportToObject(objectTransform);

            if (objectTransform.TryGetComponent(out TeleportEvent _teleportEvent))
                _teleportEvent.SetCooldown();

            SetCooldown();
        }));
    }

    private IEnumerator PauseBeforeTeleport(Action action)
    {
        yield return new WaitForSeconds(0.2f);
        anim.Play("Teleport");
        PlayerController.instance.isStopped = true;
        yield return new WaitForSeconds(1.8f);
        action();
    }

    public void SetCooldown()
    {
        isOnCooldown = true;
        Invoke(nameof(ResetCooldown), 10);
    }

    public void ResetCooldown()
    {
        isOnCooldown = false;
    }
}
