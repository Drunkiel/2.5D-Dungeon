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
    }
}
