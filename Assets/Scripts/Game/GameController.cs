using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public static bool isPC = true;
    public static bool isPaused;

    public List<GameObject> objectsToTeleportMust = new(); 
    public List<GameObject> objectsToTeleportAdditional = new();

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator LoadAsyncScene(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //Wait until scene is loaded
        while (!asyncLoad.isDone)
            yield return null;

        //Move objects to other scene
        for (int i = 0; i < objectsToTeleportMust.Count; i++)
            SceneManager.MoveGameObjectToScene(objectsToTeleportMust[i], SceneManager.GetSceneByName(sceneName));

        SceneManager.UnloadSceneAsync(currentScene);
    }
}