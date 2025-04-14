using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class SceneData
{
    public string sceneName;
    public Vector3 position;

    public SceneData(string sceneName, Vector3 position)
    {
        this.sceneName = sceneName;
        this.position = position;
    }
}

public class PortalController : MonoBehaviour
{
    public static PortalController instance;

    [SerializeField] private bool isTeleportsOnCooldown;

    [SerializeField] private SceneData _currScene;
    [SerializeField] private SceneData _prevScene;

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
            _prevScene = new(_currScene.sceneName, PlayerController.instance.transform.position);
            StartCoroutine(GameController.instance.LoadAsyncScene(sceneName));
            PopUpController.instance.CreatePopUp(PopUpInfo.VisitPlace, sceneName);
            PlayerController.instance.transform.position = playerPosition;
            _currScene = new(sceneName, playerPosition);
        }));
    }

    public void TeleportToPrevScene()
    {
        if (GameController.isPaused || string.IsNullOrEmpty(_prevScene.sceneName))
            return;

        TeleportToScene(_prevScene.sceneName, _prevScene.position);
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

    public bool IsOnCooldown() => isTeleportsOnCooldown;

    public void SetCooldown()
    {
        isTeleportsOnCooldown = true;
        Invoke(nameof(ResetCooldown), 10);
    }

    private void ResetCooldown()
    {
        isTeleportsOnCooldown = false;
    }
}
