using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent startEvents;
    public UnityEvent finishEvents;

    public void StartBattle()
    {
        StartCoroutine(ManageBattle());
    }

    public IEnumerator ManageBattle()
    {
        startEvents.Invoke();

        for (int i = 0; i < _waves.Count; i++)
        {
            StartCoroutine(SpawnWave(i));
            yield return new WaitUntil(() => _waves[i].isCompleted);
        }

        PopUpController.instance.CreatePopUp(PopUpInfo.None, "Room cleared");
        finishEvents.Invoke();
    }

    public IEnumerator SpawnWave(int index)
    {
        List<int> positionIndexes = new() { 0, 1, 2, 3 }; 

        yield return new WaitForSeconds(_waves[index].delay);
        for (int i = 0; i < _waves[index].entityIDs.Count; i++)
        {
            int a = i;
            int pIndex = Random.Range(0, positionIndexes.Count);
            GameObject newEntity = Instantiate(EntityHolder.instance.GetEntity(_waves[index].entityIDs[a], EntityAttitude.Enemy),
                                                spawnPoints[positionIndexes[pIndex]].position + new Vector3(0, 1, 0),
                                                Quaternion.identity);
            _waves[index].spawnedEntities.Add(newEntity);

            //If core add to dungeon controller
            if (_waves[index].entityIDs[a] == 1000)
                DungeonController.instance.SetCore(newEntity.GetComponent<EntityController>());

            newEntity.GetComponent<EntityController>()._statistics.onDeath.AddListener(() =>
            {
                _waves[index].spawnedEntities.Remove(newEntity);
            });

            positionIndexes.RemoveAt(pIndex);

            //If there is more entities to spawn than there are spawn points then readd positions
            if (positionIndexes.Count == 0 && a + 1 < _waves[index].entityIDs.Count)
            {
                positionIndexes = new() { 0, 1, 2, 3 };
                yield return new WaitForSeconds(_waves[index].delay);
            }
        }

        //Wait until all enemies all killed
        yield return new WaitUntil(() => _waves[index].spawnedEntities.Count == 0);
        _waves[index].isCompleted = true;
    }
}
