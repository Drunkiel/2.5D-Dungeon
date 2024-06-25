using UnityEngine;

public class TeleportEvent : MonoBehaviour
{
    [SerializeField] private bool isOnCooldown;
    public Vector3[] positions;

    public void TeleportToPosition(int positionIndex)
    {
        if (isOnCooldown)
            return;

        PortalController.instance.TeleportToPosition(positions[positionIndex]);
        SetCooldown();
    }

    public void TeleportToObject(Transform objectTransform)
    {
        if (isOnCooldown)
            return;

        PortalController.instance.TeleportToObject(objectTransform);

        if (objectTransform.TryGetComponent(out TeleportEvent _teleportEvent))
        {
            _teleportEvent.SetCooldown();
        }

        SetCooldown();
    }

    public void SetCooldown()
    {
        isOnCooldown = true;
        Invoke(nameof(ResetCooldown), 15);
    }

    public void ResetCooldown()
    {
        isOnCooldown = false;
    }
}
