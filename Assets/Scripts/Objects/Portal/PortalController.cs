using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void TeleportToScene(int sceneIndex)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(() =>
        {
            if (SceneManager.sceneCount < sceneIndex)
                return;

            SceneManager.LoadScene(sceneIndex);
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
