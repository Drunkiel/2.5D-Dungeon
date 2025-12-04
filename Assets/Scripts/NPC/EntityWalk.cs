using System.Collections;
using UnityEngine;

public class EntityWalk : MonoBehaviour
{
    public bool isStopped;

    private Vector3 startPosition;
    [SerializeField] private float wanderingDistance;
    [SerializeField] private Vector3 positionToGo;
    public float allowedDistance;
    public Transform targetTransform;

    public Vector2 movement;
    public bool isMoving;
    [SerializeField] private float movingTime;

    private bool isFlipped;
    public bool isFacingCamera;
    private bool isNewPositionFound;

    [HideInInspector] public Vector3 move;
    private EntityController _controller;

    private void Start()
    {
        _controller = GetComponent<EntityController>();

        //Setting first position to go
        startPosition = transform.position;
        SetNewPosition(wanderingDistance);
    }

    private void Update()
    {
        isMoving = movement.magnitude > 0.01f;

        if (isStopped || GameController.isPaused)
            return;

        //Check if entity is moving
        if (isMoving)
            movingTime += Time.deltaTime;
        else if (movingTime != 0)
            movingTime = 0f;

        if (isMoving && movingTime > 0.2f && _controller.rgBody.velocity.magnitude < 0.01f)
            Jump();
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            print($"Entity fall off the map {transform.name} on position: {transform.position}");
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

    public void ApproachTarget()
    {
        Vector3 targetPosition = targetTransform != null ? targetTransform.position : transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > allowedDistance)
            positionToGo = targetPosition;
        else
            positionToGo = transform.position;

        GoTo(positionToGo, targetPosition);
    }

    public void FleeFromTarget()
    {
        Vector3 targetPosition = targetTransform != null ? targetTransform.position : PlayerController.instance.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

        if (distanceToPlayer < allowedDistance)
        {
            Vector3 directionAwayFromPlayer = (transform.position - targetPosition).normalized;

            Vector3 newPosition = transform.position + directionAwayFromPlayer * (allowedDistance - distanceToPlayer);
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
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        float newFaceDirection = Vector3.Dot(cameraRight, faceDirection - transform.position);

        movement = new Vector2(
            direction.x * _controller._statistics.speedForce,
            direction.z * _controller._statistics.speedForce
        );

        //Check if entity moves towards camera
        Vector3 toCamera = Camera.main.transform.position - transform.position;
        toCamera.y = 0;
        toCamera.Normalize();

        Vector3 moveDir = direction;
        moveDir.y = 0;
        moveDir.Normalize();

        float dotToCamera = Vector3.Dot(moveDir, toCamera);

        //Flipping Entity to direction they are going based on camera view
        if (dotToCamera > 0 && !isFacingCamera)
        {
            isFacingCamera = true;
            GetComponent<EntityLookController>().UpdateEntityLookAll(isFacingCamera);
            GetComponent<EntityLookController>().RotateCharacter(!isFlipped, isFacingCamera);
        }
        else if (dotToCamera < 0 && isFacingCamera)
        {
            isFacingCamera = false;
            GetComponent<EntityLookController>().UpdateEntityLookAll(isFacingCamera);
            GetComponent<EntityLookController>().RotateCharacter(!isFlipped, isFacingCamera);
        }

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

    private void Jump()
    {
        _controller.rgBody.velocity = new Vector3(_controller.rgBody.velocity.x, 0f, _controller.rgBody.velocity.z);
        _controller.rgBody.AddForce(transform.up * _controller._statistics.jumpForce, ForceMode.Impulse);
    }
}
