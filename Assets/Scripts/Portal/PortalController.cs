using System.Collections;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public static PortalController instance;

    private void Awake()
    {
        instance = this;
    }

    public void TeleportToPosition(Vector3 position)
    {
        if (PlayerController.instance.isPlayerStopped)
            return;

        StartCoroutine(WaitAndTeleport(position));
    }

    public void TeleportToObject(Transform objectTransform)
    {
        if (PlayerController.instance.isPlayerStopped)
            return;

        StartCoroutine(WaitAndTeleport(objectTransform.position));
    }

    IEnumerator WaitAndTeleport(Vector3 position)
    {
        //Some effects before teleportation
        PlayerController.instance.isPlayerStopped = true;
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);
        TransitionController.instance.StartTransition();

        yield return new WaitForSeconds(1);

        //Teleporting player
        PlayerController.instance.transform.position = position;
        PlayerController.instance.ResetMovement();
        PlayerController.instance.isPlayerStopped = false;
        CameraController.instance.ResetZoom();
    }
}
