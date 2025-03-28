using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public bool isCompleted;
    public List<short> entityIDs = new();
    public float delay = 0.5f;
    public List<GameObject> spawnedEntities = new();
}

public class BattleRoom : MonoBehaviour
{
    public List<Transform> spawnPoints = new();
    public List<Wave> _waves = new();

    private void Start()
    {
        StartCoroutine(SpawnWave(0));
    }

    public IEnumerator SpawnWave(int index)
    {
        List<int> positionIndexes = new() { 0, 1, 2, 3 }; 

        yield return new WaitForSeconds(_waves[index].delay);
        for (int i = 0; i < _waves[index].entityIDs.Count; i++)
        {
            int a = i;
            int pIndex = Random.Range(0, positionIndexes.Count);
            GameObject newEntity = Instantiate(EntityHolder.instance.GetEntity(_waves[index].entityIDs[a], EntityAttitude.Enemy), spawnPoints[positionIndexes[pIndex]].position + new Vector3(0, 1, 0), Quaternion.identity);
            _waves[index].spawnedEntities.Add(newEntity);
            positionIndexes.RemoveAt(pIndex);

            //If there is more entities to spawn than there are spawn points then readd positions
            if (positionIndexes.Count == 0 && a + 1 < _waves[index].entityIDs.Count)
            {
                positionIndexes = new() { 0, 1, 2, 3 };
                yield return new WaitForSeconds(_waves[index].delay);
            }
        }
    }
}
