using System.Collections;
using UnityEngine;

public class EntityWalk : MonoBehaviour
{
    public bool isStopped;

    private Vector3 startPosition;
    [SerializeField] private float wanderingDistance;
    [SerializeField] private Vector3 positionToGo;

    public Vector2 movement;
    public bool isMoving;

    private bool isFlipped;
    private bool isNewPositionFound;

    [HideInInspector] public Vector3 move;
    private EntityController _controller;

    private void Start()
    {
        _controller = GetComponent<EntityController>();

        //Setting new first position to go
        startPosition = transform.position;
        SetNewPosition(wanderingDistance);
    }

    private void Update()
    {
        isMoving = movement.magnitude > 0.01f;

        //if (isStopped || GameController.isPaused)
        //    return;
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            print($"Enemy fall off the map {transform.name} on position: {transform.position}");
            Destroy(gameObject);
        }

        if (isStopped || GameController.isPaused)
            return;

        move = new Vector3(movement.x, 0, movement.y).normalized;
    }

    public void Patrolling()
    {
        //Checking distance to new position
        if (Vector3.Distance(transform.position, new(positionToGo.x, transform.position.y, positionToGo.z)) < 0.2f)
        {
            if (!isNewPositionFound)
            {
                StartCoroutine(PauseBeforeNewPosition());
                isNewPositionFound = true;
            }
            else
                movement = Vector2.zero;
        }
        else
            GoTo(positionToGo, positionToGo);
    }

    //Make it to be depended by skill
    private const float AllowedDistance = 1.0f;
    private const float TooCloseDistance = 0.5f;

    public void ApproachPlayer()
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        if (distanceToPlayer > AllowedDistance)
            positionToGo = playerPosition;
        else
            positionToGo = transform.position;

        GoTo(positionToGo, playerPosition);
    }

    public void FleeFromPlayer()
    {
        Vector3 playerPosition = PlayerController.instance.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);

        if (distanceToPlayer < TooCloseDistance)
        {
            Vector3 directionAwayFromPlayer = (transform.position - playerPosition).normalized;

            Vector3 newPosition = transform.position + directionAwayFromPlayer * (TooCloseDistance - distanceToPlayer);
            positionToGo = newPosition;
        }
        else
            positionToGo = transform.position;

        GoTo(positionToGo, positionToGo);
    }

    public void RandomRun()
    {
        //Checking distance to new position
        if (Vector3.Distance(transform.position, positionToGo) < 1f)
            SetNewPosition(wanderingDistance * 2);
        else
            GoTo(positionToGo, positionToGo);
    }

    private void SetNewPosition(float distance)
    {
        positionToGo = GetRandomPosition(distance);
    }

    private IEnumerator PauseBeforeNewPosition()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        isNewPositionFound = false;
        SetNewPosition(wanderingDistance);
    }

    public void GoTo(Vector3 position, Vector3 faceDirection)
    {
        Vector3 direction = position - transform.position;
        float newFaceDirection = (faceDirection - transform.position).x;
        movement = new Vector2(
            direction.x * _controller._statistics.speedForce,
            direction.z * _controller._statistics.speedForce
        );

        //Flipping Enemy to direction they are going
        if (newFaceDirection < 0 && !isFlipped)
        {
            transform.GetChild(0).localScale = new(-1, 1, 1);
            isFlipped = true;
        }
        else if (newFaceDirection > 0 && isFlipped)
        {
            transform.GetChild(0).localScale = new(1, 1, 1);
            isFlipped = false;
        }
    }

    private Vector3 GetRandomPosition(float distance)
    {
        //Getting random position on X and Z axis
        float deltaX = Random.Range(-distance, distance);
        float deltaZ = Random.Range(-distance, distance);

        //New position
        Vector3 newPosition = new(startPosition.x + deltaX, startPosition.y, startPosition.z + deltaZ);
        return newPosition;
    }
}