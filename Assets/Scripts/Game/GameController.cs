using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
        if (gameObject.scene.name != "DontDestroyOnLoad")
            DontDestroyOnLoad(gameObject);
    }
}
