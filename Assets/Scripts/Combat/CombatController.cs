using System;
using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    [SerializeField] private bool isPlayerTurn;
    public Transform playerPlace;
    public Transform enemyPlace;
    public Transform cameraLookPoint;
    public CombatUI _combatUI;
    [SerializeField] private OpenCloseUI _openCloseUI;

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
        isPlayerTurn = true;

        StartCoroutine(WaitAndLoadScene());
        StartCoroutine(WaitAndSetForCombat());
    }

    public void EndCombat()
    {
        StartCoroutine(WaitAndLoadScene());
        StartCoroutine(WaitAndReset());
    }

    public void TakeTurn(Action action)
    {
        //Do some animation stuff

        //Taking action
        action();

        //Checks
        CombatEntities _combatEntities = CombatEntities.instance;
        if (_combatEntities.player.GetComponent<PlayerController>()._statistics.health <= 0 || 
            _combatEntities.enemy.GetComponent<EnemyController>()._statistics.health <= 0)
            EndCombat();

        //Ending turn
        isPlayerTurn = !isPlayerTurn;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    IEnumerator WaitAndLoadScene()
    {
        //Some effects before starting battle
        PlayerController.instance.isPlayerStopped = true;
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);

        //Starting transition
        TransitionController.instance.StartTransition();

        yield return new WaitForSeconds(1);
    }

    IEnumerator WaitAndSetForCombat()
    {
        //Set player skills
        for (int i = 0; i < 6; i++)
            _combatUI.SetSkillToBTN(i, PlayerController.instance._holdingController._skillsController._skillHolder._skillDatas[i]);

        yield return new WaitForSeconds(2);

        //Setting player to combat state
        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = playerPlace.position;
        _combatEntities.player.transform.GetChild(0).localScale = new(1, 1, 1);

        //Setting enemy to combat state
        _combatEntities.enemy.transform.position = enemyPlace.position;
        _combatEntities.enemy.transform.GetChild(0).localScale = new(-1, 1, 1);

        //Setting camera to combat state
        CameraController.instance.ResetZoom();
        CameraController.instance.SetCamera(1);
        _openCloseUI.Open();
    }

    IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(2);

        //Reseting player to previous state
        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = _combatEntities.playerPreviousPosition;
        _combatEntities.player.transform.GetChild(0).localScale = new(_combatEntities.playerXScale, 1, 1);
        PlayerController.instance.ResetMovement();
        PlayerController.instance.isPlayerStopped = false;

        //Destroying enemy
        Destroy(_combatEntities.enemy);

        //Reseting camera and UI
        CameraController.instance.ResetZoom();
        CameraController.instance.SetCamera(0);
        _openCloseUI.Close();
    }
}
