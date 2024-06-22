using UnityEngine;

public class SpawnEntities : MonoBehaviour
{
    [SerializeField] private Transform playerParent;
    [SerializeField] private Transform enemyParent;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        print(CombatEntities.instance.enemy);

        //Instantiate(CombatEntities.player, playerParent);
        //Instantiate(CombatEntities.enemy, enemyParent);
    }
}
