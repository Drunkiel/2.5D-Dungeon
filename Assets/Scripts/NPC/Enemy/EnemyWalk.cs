using System.Collections;
using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    public bool isStopped;

    private Vector3 startPosition;
    [SerializeField] private float wanderingDistance;
    private Vector3 positionToGo;

    public Vector2 movement;
    public bool isMoving;

    private bool isFlipped;
    private bool isNewPositionFound;

    [HideInInspector] public Vector3 move;
    private EnemyController _controller;

    private void Start()
    {
        _controller = GetComponent<EnemyController>();

        //Setting new first position to go
        startPosition = transform.position;
        SetNewPosition();
    }

    private void Update()
    {
        isMoving = movement.magnitude > 0.01f;

        if (isStopped || GameController.isPaused)
            return;
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
        if (Vector3.Distance(transform.position, positionToGo) < 0.2f)
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
            GoTo(positionToGo);
    }

    private void SetNewPosition()
    {
        positionToGo = GetRandomPosition();
    }

    private IEnumerator PauseBeforeNewPosition()
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        isNewPositionFound = false;
        SetNewPosition();
    }

    public void GoTo(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        movement = new Vector2(
            direction.x * _controller._statistics.speedForce,
            direction.z * _controller._statistics.speedForce
        );

        //Flipping Enemy to direction they are going
        if (movement.x < 0 && !isFlipped)
        {
            transform.GetChild(0).localScale = new(-1, 1, 1);
            isFlipped = true;
        }
        else if (movement.x > 0 && isFlipped)
        {
            transform.GetChild(0).localScale = new(1, 1, 1);
            isFlipped = false;
        }
    }

    private Vector3 GetRandomPosition()
    {
        //Getting random position on X and Z axis
        float deltaX = Random.Range(-wanderingDistance, wanderingDistance);
        float deltaZ = Random.Range(-wanderingDistance, wanderingDistance);

        //New position
        Vector3 newPosition = new(startPosition.x + deltaX, startPosition.y, startPosition.z + deltaZ);
        return newPosition;
    }
}
