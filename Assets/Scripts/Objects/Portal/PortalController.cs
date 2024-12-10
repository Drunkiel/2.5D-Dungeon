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
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(position));
    }

    public void TeleportToObject(Transform objectTransform)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(objectTransform.position));
    }

    IEnumerator WaitAndTeleport(Vector3 position)
    {
        //Some effects before teleportation
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);
        TransitionController.instance.StartTransition(1);

        yield return new WaitForSeconds(1);

        //Teleporting player
        PlayerController.instance.transform.position = position;
        PlayerController.instance.ResetMovement();
        PlayerController.instance.isStopped = false;
        CameraController.instance.ResetZoom();
    }
}
