using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PortalSpawn
{
    public GameObject portalObject;
    public Vector3 position;
}

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

    public List<PortalSpawn> _spawnList = new();

    [SerializeField] private bool isTeleportsOnCooldown;

    public SceneData _currScene;
    [SerializeField] private SceneData _prevScene;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnPortal(int index)
    {
        if (_spawnList.Count <= index)
            return;

        Instantiate(_spawnList[index].portalObject, _spawnList[index].position, Quaternion.identity);
    }

    public void TeleportToPosition(Vector3 position)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(() =>
        {
            GameController.instance._player.transform.position = position;
        }));
    }

    public void TeleportToObject(Transform objectTransform)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(() =>
        {
            GameController.instance._player.transform.position = objectTransform.position;
        }));
    }

    public void TeleportToScene(string sceneName, Vector3 playerPosition)
    {
        if (GameController.isPaused)
            return;

        StartCoroutine(WaitAndTeleport(() =>
        {
            _prevScene = new(_currScene.sceneName, GameController.instance._player.transform.position);
            StartCoroutine(GameController.instance.LoadAsyncScene(sceneName));
            PopUpController.instance.CreatePopUp(PopUpInfo.VisitPlace, sceneName);
            GameController.instance._player.transform.position = playerPosition;
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
        GameController.instance._player.GetComponent<PlayerMovement>().ResetMovement();
        GameController.instance._player.StopEntity(false);
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
