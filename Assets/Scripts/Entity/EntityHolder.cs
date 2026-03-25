using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public EntityController _entityController;
}

public class EntityHolder : MonoBehaviour
{
    public static EntityHolder instance;

    public List<Entity> _entitiesFriendly = new();
    public List<Entity> _entitiesEnemy = new();

    private void Awake()
    {
        instance = this;
    }

    public EntityController GetEntity(short id, EntityAttitude entityAttitude)
    {
        return entityAttitude switch
        {
            EntityAttitude.Friendly => GetFriendlyEntity(id),
            EntityAttitude.Enemy => GetEnemyEntity(id),
            _ => null,
        };
    }

    public EntityController GetEntityInScene(short id)
    {
        EntityController[] _entityControllers = (EntityController[])FindObjectsOfType(typeof(EntityController));
        foreach (EntityController _entityController in _entityControllers)
        {
            if (_entityController._entityInfo.ID == id)
                return _entityController;
        }

        return null;
    }

    private EntityController GetFriendlyEntity(short id)
    {
        if (id >= 1000)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, "Friendly entity's id is lower than 1000");
            return null;
        }

        if (_entitiesFriendly.Count <= id)
            return null;

        return _entitiesFriendly[id]._entityController;
    }

    private EntityController GetEnemyEntity(short id)
    {
        if (id < 1000)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, "Enemy id's starts over 1000+");
            return null;
        }

        short newID = (short)(id - 1000);
        if (_entitiesEnemy.Count <= newID)
            return null;

        return _entitiesEnemy[newID]._entityController;
    }
}
