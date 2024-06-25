using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartCombat()
    {
        StartCoroutine(WaitAndLoadScene(1));
    }

    public void EndCombat()
    {
        StartCoroutine(WaitAndLoadScene(0));
    }

    IEnumerator WaitAndLoadScene(int sceneIndex)
    {
        //Some effects before starting battle
        PlayerController.instance.isPlayerStopped = true;
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);
        TransitionController.instance.StartTransition();

        yield return new WaitForSeconds(1);

        //Loading combat scene
        SceneManager.LoadScene(sceneIndex);

        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = new(-1.7f, 2, 0);
        _combatEntities.enemy.transform.position = new(1.7f, 2f, 0);
        _combatEntities.enemy.transform.GetChild(0).localScale = new(-1, 1, 1);
    }
}
