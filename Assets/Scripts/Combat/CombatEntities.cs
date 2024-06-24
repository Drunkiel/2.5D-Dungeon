using UnityEngine;

public class CombatEntities : MonoBehaviour
{
    public static CombatEntities instance;

    public GameObject player;
    public GameObject enemy;

    private void Awake()
    {
        instance = this;
    }
}
