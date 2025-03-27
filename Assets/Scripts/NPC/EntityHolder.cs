using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public GameObject entityObject;
}

public class EntityHolder : MonoBehaviour
{
    public List<Entity> _entitiesFriendly = new();
    public List<Entity> _entitiesEnemy= new();

    public GameObject GetFriendlyEntity(short id)
    {
        return null;
    }

    public GameObject GetEnemyEntity(short id)
    {
        return null;
    }
}
