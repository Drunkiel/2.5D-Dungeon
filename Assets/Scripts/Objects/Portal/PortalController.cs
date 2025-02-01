using System;
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

        StartCoroutine(WaitAndTeleport(() =>
        {
            PlayerController.instance.transform.position = position;
        }));
    }

    public void TeleportToObject(Transform objectTransform)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(() =>
        {
            PlayerController.instance.transform.position = objectTransform.position;
        }));
    }

    public void TeleportToScene(string sceneName, Vector3 playerPosition)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(() =>
        {
            StartCoroutine(GameController.instance.LoadAsyncScene(sceneName));
            PopUpController.instance.CreatePopUp(PopUpInfo.VisitPlace, sceneName);
            PlayerController.instance.transform.position = playerPosition;
        }));
    }

    IEnumerator WaitAndTeleport(Action action)
    {
        //Some effects before teleportation
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);
        TransitionController.instance.StartTransition(1);

        yield return new WaitForSeconds(1);

        //Teleporting player
        action();
        PlayerController.instance.ResetMovement();
        PlayerController.instance.isStopped = false;
        CameraController.instance.ResetZoom();
    }
}
