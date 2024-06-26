using UnityEngine;

public class CombatEntities : MonoBehaviour
{
    public static CombatEntities instance;

    public GameObject player;
    public GameObject enemy;

    public Vector3 playerPreviousPosition;
    public float playerXScale;

    private void Awake()
    {
        instance = this;
    }
}
