using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

public class EnemyController : MonoBehaviour
{
    public EntityStatistics _statistics;
    public HoldingController _holdingController;

    public State currentState;
    public List<BehaviourState> _behaviourStates = new();

    [SerializeField] private Rigidbody rgBody;
    public Animator anim;

    public EnemyWalk _enemyWalk;

    private void Start()
    {
        _statistics.SaveStats();

        //Setting sliders value
        _statistics._statsController.UpdateHealthSlider(_statistics.health, _statistics.maxHealth);
        _statistics._statsController.UpdateManaSlider(_statistics.mana, _statistics.maxMana);

        StartCoroutine(AutoRegen());
    }

    private void Update()
    {
        if (_enemyWalk.isStopped || GameController.isPaused)
            return;

        switch (currentState)
        {
            case State.Standing:
                _behaviourStates[0].acton.Invoke();
                break;

            case State.Patroling:
                _behaviourStates[1].acton.Invoke();
                break;

            case State.Attacking:
                _behaviourStates[2].acton.Invoke();
                break;
        }

        //Updating buffs
        _statistics.UpdateBuffs(_holdingController._itemController._gearHolder);

        //Controls max speed
        if (rgBody.velocity.magnitude > _statistics.maxSpeed)
            rgBody.velocity = Vector3.ClampMagnitude(rgBody.velocity, _statistics.maxSpeed);
    }

    private void FixedUpdate()
    {
        if (_enemyWalk.isStopped || GameController.isPaused)
            return;

        rgBody.AddForce(_enemyWalk.move * (_statistics.speedForce * rgBody.mass), ForceMode.Force);
    }

    private IEnumerator AutoRegen()
    {
        //Wait 0.5s then regen some hp and mana
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
}
