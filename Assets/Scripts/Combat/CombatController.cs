using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public Transform playerPlace;
    public Transform enemyPlace;
    public Transform cameraLookPoint;
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

        //Starting transition
        TransitionController.instance.StartTransition();

        yield return new WaitForSeconds(1);
    }

    IEnumerator WaitAndSetForCombat()
    {
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
