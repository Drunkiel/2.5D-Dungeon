using System;
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
        //Setting sliders value
        _statistics._statsController.UpdateHealthSlider(_statistics.health, _statistics.maxHealth);
        _statistics._statsController.UpdateManaSlider(_statistics.mana, _statistics.maxMana);
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

}
