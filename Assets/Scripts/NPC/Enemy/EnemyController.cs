using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public bool isStopped;
    public EntityStatistics _statistics;
    public HoldingController _holdingController;

    public Vector2 movement;
    public bool isMoving;

    [SerializeField] private Rigidbody rgBody;
    public Animator anim;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float wanderingDistance;
    [SerializeField] private Vector3 positionToGo;

    private bool isFlipped;
    private bool isNewPositionFound;

    private void Start()
    {
        //Setting sliders value
        _statistics._statsController.UpdateHealthSlider(_statistics.health);
        _statistics._statsController.UpdateManaSlider(_statistics.mana);

        //Setting new first position to go
        startPosition = transform.position;
        SetNewPosition();
    }

    private void Update()
    {
        isMoving = movement.magnitude > 0.01f;

        if (isStopped || GameController.isPaused)
            return;

        //Updating buffs
        _statistics.UpdateBuffs(_holdingController._itemController._gearHolder);

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

        //Controls max speed
        if (rgBody.velocity.magnitude > _statistics.maxSpeed)
            rgBody.velocity = Vector3.ClampMagnitude(rgBody.velocity, _statistics.maxSpeed);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            print($"Enemy fall off the map {transform.name} on position: {transform.position}");
            Destroy(gameObject);
        }

        if (isStopped|| GameController.isPaused)
            return;

        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        rgBody.AddForce(move * (_statistics.speedForce * rgBody.mass), ForceMode.Force);
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
            direction.x * _statistics.speedForce,
            direction.z * _statistics.speedForce
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
