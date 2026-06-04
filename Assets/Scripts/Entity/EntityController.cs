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
    public EntityStats _statistics;

    public bool isStopped
    {
        get;
        private set;
    }
    private int stopCounter;
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

    public ItemController _itemController;
    public SkillController _skillsController;

    public Rigidbody rgBody;
    public Animator anim;

    private float nextCheck = 0;

    private void Start()
    {
        isFacingCamera = true;
        _statistics.SaveStats();
        _statistics.vitals.Initialize();
        _itemController.OnGearChanged += () => _statistics.RecalculateStatistics(_itemController._gearHolder);

        //Setting basic info of entity
        _statistics._statsController.SetName(_entityInfo);
        _statistics._statsController.SetSliderColor(_entityInfo.entity);

        //Setting sliders value
        _statistics._statsController.UpdateHealthSlider(_statistics.vitals.health, _statistics.vitals.maxHealth.Value, true);
        _statistics._statsController.UpdateManaSlider(_statistics.vitals.mana, _statistics.vitals.maxMana.Value, true);

        StartCoroutine(AutoRegen());
    }

    private void Update()
    {
        if (isStopped || GameController.isPaused)
            return;

        //Updating buffs / Check every second
        if (Time.time >= nextCheck)
        {
            if (_statistics._activeBuffs.Count > 0)
                _statistics.UpdateBuffs(_itemController._gearHolder);
            nextCheck = Time.time + 1f;
        }
    }

    public void StopEntity(bool value)
    {
        if (value)
            stopCounter++;
        else
            stopCounter = Mathf.Max(0, stopCounter - 1);

        isStopped = stopCounter > 0;
    }
    public void Flip(bool value) => isFlipped = value;
    public void FaceCamera(bool value) => isFacingCamera = value;

    private IEnumerator AutoRegen()
    {
        //Wait then regenerate some hp and mana
        yield return new WaitForSeconds(1f);

        if (!GetComponent<EntityCombat>().inCombat)
        {
            if (_statistics.vitals.health < _statistics.vitals.maxHealth.Value)
                _statistics.TakeDamage(-_statistics.vitals.healthRegeneration, AttributeTypes.Buff, ElementType.None, _statistics, true);

            if (_statistics.vitals.mana < _statistics.vitals.maxMana.Value)
                _statistics.TakeMana(-_statistics.vitals.manaRegeneration, true);
        }

        StartCoroutine(AutoRegen());
    }

    public void KillEntity()
    {
        QuestController.instance.InvokeKillEvent(_entityInfo.ID);
        Destroy(gameObject);
    }
}
