using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public Transform playerPlace;
    public Transform enemyPlace;
    public Transform cameraLookPoint;

    private void Awake()
    {
        instance = this;
    }

    public void StartCombat()
    {
        CombatEntities _combatEntities = CombatEntities.instance;
        PlayerController _playerController = PlayerController.instance;

        _combatEntities.playerPreviousPosition = _playerController.transform.position;
        _combatEntities.playerXScale = _playerController.transform.localScale.x;

        StartCoroutine(WaitAndLoadScene());
        StartCoroutine(WaitAndSetForCombat());
    }

    public void EndCombat()
    {
        StartCoroutine(WaitAndLoadScene());
        StartCoroutine(WaitAndReset());
    }

    IEnumerator WaitAndLoadScene()
    {
        //Some effects before starting battle
        PlayerController.instance.isPlayerStopped = true;
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);

        TransitionController.instance.StartTransition();

        yield return new WaitForSeconds(1);
    }

    IEnumerator WaitAndSetForCombat()
    {
        yield return new WaitForSeconds(2);

        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = playerPlace.position;
        _combatEntities.player.transform.GetChild(0).localScale = new(1, 1, 1);

        _combatEntities.enemy.transform.position = enemyPlace.position;
        _combatEntities.enemy.transform.GetChild(0).localScale = new(-1, 1, 1);

        CameraController.instance.ResetZoom();
        CameraController.instance.virtualCameras[1].Priority = 99;
    }

    IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(2);

        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = _combatEntities.playerPreviousPosition;
        _combatEntities.player.transform.GetChild(0).localScale = new(_combatEntities.playerXScale, 1, 1);

        CameraController.instance.ResetZoom();
        CameraController.instance.virtualCameras[1].Priority = 1;
    }
}
