using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public GameObject entityObject;
}

public class EntityHolder : MonoBehaviour
{
    public static EntityHolder instance;

    public List<Entity> _entitiesFriendly = new();
    public List<Entity> _entitiesEnemy= new();

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetEntity(short id, EntityAttitude entityAttitude)
    {
        return entityAttitude switch
        {
            EntityAttitude.Friendly => GetFriendlyEntity(id),
            EntityAttitude.Enemy => GetEnemyEntity(id),
            _ => null,
        };
    }

    private GameObject GetFriendlyEntity(short id)
    {
        if (_entitiesFriendly.Count <= id)
            return null;

        return _entitiesFriendly[id].entityObject;
    }

    private GameObject GetEnemyEntity(short id)
    {
        if (_entitiesEnemy.Count <= id)
            return null;

        return _entitiesEnemy[id].entityObject;
    }
}
