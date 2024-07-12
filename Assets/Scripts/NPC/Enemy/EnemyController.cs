using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EntityStatistics _statistics;
    public HoldingController _holdingController;

    public Vector2 movement;
    public bool isMoving;

    [SerializeField] private Rigidbody rgBody;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private int wanderingDistance;
    [SerializeField] private Vector3 positionToGo;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        isMoving = movement.magnitude > 0.01f;
        GoTo(positionToGo);

        if (rgBody.velocity.magnitude > _statistics.maxSpeed)
            rgBody.velocity = Vector3.ClampMagnitude(rgBody.velocity, _statistics.maxSpeed);
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        rgBody.AddForce(move * _statistics.speedForce, ForceMode.Acceleration);

        if (Vector3.Distance(transform.position, positionToGo) < 1)
            positionToGo = GetRandomPosition();
    }

    private void GoTo(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        movement = new Vector2(
            direction.x * _statistics.speedForce,
            direction.z * _statistics.speedForce
        );
    }

    private Vector3 GetRandomPosition()
    {
        //Getting random position on X and Z axis
        float deltaX = Random.Range(-wanderingDistance / 2, wanderingDistance / 2);
        float deltaZ = Random.Range(-wanderingDistance / 2, wanderingDistance / 2);

        //New position
        Vector3 newPosition = new(startPosition.x + deltaX, startPosition.y, startPosition.z + deltaZ);
        return newPosition;
    }
}
