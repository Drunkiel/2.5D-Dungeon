using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public EntityController _player;
    [SerializeField] private Canvas mainCanvas;

    public static bool isPC = true;
    public static bool isPaused;

    public List<GameObject> objectsToTeleportMust = new();
    public List<GameObject> objectsToTeleportAdditional = new();

    [SerializeField] private TMP_Text versionText;

    private void Awake()
    {
        instance = this;

        string version =
            $"Developed by <color=yellow>Drunkiel</color>\n" +
            $"Version: <color=yellow>{Application.version}</color>";

        versionText.text = version;
    }

    public IEnumerator LoadAsyncScene(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //Wait until scene is loaded
        while (!asyncLoad.isDone)
            yield return null;

        Scene nextScene = SceneManager.GetSceneByName(sceneName);

        //Move objects to other scene
        for (int i = 0; i < objectsToTeleportMust.Count; i++)
            SceneManager.MoveGameObjectToScene(objectsToTeleportMust[i], nextScene);

        SceneManager.UnloadSceneAsync(currentScene);
    }

    public Canvas GetCanvas() => mainCanvas;
}