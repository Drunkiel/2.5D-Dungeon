using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TeleportEvent : MonoBehaviour
{
    [SerializeField] private bool isOnCooldown;
    [SerializeField] private float cooldownToTeleport = 2f;
    public Vector3[] positions;
    [SerializeField] private Animator anim;

    public UnityEvent onTeleportEvent;

    public void TeleportToPosition(int positionIndex)
    {
        if (isOnCooldown || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
            return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            onTeleportEvent.Invoke();
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
            if (objectTransform.TryGetComponent(out TeleportEvent _teleportEvent))
                _teleportEvent.SetCooldown();
            SetCooldown();

            onTeleportEvent.Invoke();
            PortalController.instance.TeleportToObject(objectTransform);
        }));
    }

    public void TeleportToScene(string sceneName)
    {
        if (isOnCooldown || PlayerController.instance.GetComponent<EntityCombat>().inCombat)
            return;

        StartCoroutine(PauseBeforeTeleport(() =>
        {
            //Basic string verificacion
            if (string.IsNullOrEmpty(sceneName))
            {
                ConsoleController.instance.ChatMessage(SenderType.System, $"Scene named: {sceneName} is not found");
                return;
            }

            //Check if position exists
            if (positions.Length < 1)
                positions = new Vector3[1] { Vector3.zero };

            onTeleportEvent.Invoke();
            PortalController.instance.TeleportToScene(sceneName, positions[0]);
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
