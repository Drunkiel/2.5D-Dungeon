using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum EntityAttitude
{
    Friendly,
    Enemy,
}

public enum Behaviour
{
    Passive,
    Neutral,
    Aggresive,
}

public enum EntityClass
{
    None,
    Warrior,
    Archer,
    Mage
}

public enum State
{
    Standing,
    Patroling,
    Attacking,
}

[Serializable]
public class BehaviourState
{
    public State state;
    public UnityEvent acton;
}

public class EntityController : MonoBehaviour
{
    public EntityInfo _entityInfo;
    public EntityStatistics _statistics;

    public bool isStopped
    {
        get;
        private set;
    }
    public bool isFlipped
    {
        get;
        private set;
    }
    public bool isFacingCamera
    {
        get;
        private set;
    }

    public HoldingController _holdingController;

    public Rigidbody rgBody;
    public Animator anim;

    private void Start()
    {
        isFacingCamera = true;
        _statistics.SaveStats();

        //Setting basic info of entity
        _statistics._statsController.SetName(_entityInfo);
        _statistics._statsController.SetSliderColor(_entityInfo.entity);

        //Setting sliders value
        _statistics._statsController.UpdateHealthSlider(_statistics.health, _statistics.maxHealth, true);
        _statistics._statsController.UpdateManaSlider(_statistics.mana, _statistics.maxMana, true);

        StartCoroutine(AutoRegen());
    }

    private void Update()
    {
        if (isStopped || GameController.isPaused)
            return;

        //Updating buffs
        _statistics.UpdateBuffs(_holdingController._itemController._gearHolder);
    }

    public void StopEntity(bool value) => isStopped = value;
    public void Flip(bool value) => isFlipped = value;
    public void FaceCamera(bool value) => isFacingCamera = value;

    private IEnumerator AutoRegen()
    {
        //Wait then regen some hp and mana
        yield return new WaitForSeconds(1f);

        if (!GetComponent<EntityCombat>().inCombat)
        {
            if (_statistics.health < _statistics.maxHealth)
                _statistics.TakeDamage(-_statistics.healthRegeneration, AttributeTypes.Buff, ElementalTypes.NoElement, true);

            if (_statistics.mana < _statistics.maxMana)
                _statistics.TakeMana(-_statistics.manaRegeneration, true);
        }

        StartCoroutine(AutoRegen());
    }

    public void KillEntity()
    {
        QuestController.instance.InvokeKillEvent(_entityInfo.ID);
        Destroy(gameObject);
    }
}
