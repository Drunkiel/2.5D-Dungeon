using System.Collections.Generic;
using UnityEngine;

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
}
