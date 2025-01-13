using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportEvent : MonoBehaviour
{
    [SerializeField] private bool isOnCooldown;
    [SerializeField] private float cooldownToTeleport = 2f;
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

    public void TeleportToScene(int sceneIndex)
    {
        if (isOnCooldown || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
            return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            PortalController.instance.TeleportToScene(sceneIndex);
            SetCooldown();
        }));
    }

    private IEnumerator PauseBeforeTeleport(Action action)
    {
        yield return new WaitForSeconds(cooldownToTeleport * 0.1f);
        if (anim != null)
            anim.Play("Teleport");
        PlayerController.instance.isStopped = true;
        yield return new WaitForSeconds(cooldownToTeleport * 0.9f);
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
