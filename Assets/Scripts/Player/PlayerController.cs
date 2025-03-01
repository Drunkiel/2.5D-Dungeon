using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public EntityStatistics _statistics;

    public bool isStopped;

    public float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] private bool grounded;
    private bool isFlipped;

    [SerializeField] private Vector3 lastGroundedPosition;

    private Vector2 movement;
    private Vector2 newVelocityXZ;
    private float newVelocityY;
    public bool isMoving;

    public HoldingController _holdingController;
    [SerializeField] private Rigidbody rgBody;
    public Animator anim;
    [SerializeField] private ParticleSystem particle;

    private AudioSource moveSoundSource;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _statistics.SaveStats();
        moveSoundSource = GetComponent<AudioSource>();
        _statistics._statsController.UpdateHealthSlider(_statistics.health, _statistics.maxHealth);
        _statistics._statsController.UpdateManaSlider(_statistics.mana, _statistics.maxMana);
        StartCoroutine(AutoRegen());
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if player is moving and touching ground
        isMoving = movement.magnitude > 0.1f;
        grounded = Physics.Raycast(transform.position + new Vector3(0.05f, 0), Vector3.down, playerHeight, whatIsGround) || 
            Physics.Raycast(transform.position + new Vector3(-0.05f, 0), Vector3.down, playerHeight, whatIsGround);

        anim.SetFloat("Movement", isStopped ? 0 : movement.magnitude);

        if (isStopped || GameController.isPaused)
            return;

        //Updating buffs
        _statistics.UpdateBuffs(_holdingController._itemController._gearHolder);

        // if (isMoving)
        // {
        //     particle.Play();
        // }

        //Clamping movement speed
        newVelocityXZ = new(rgBody.velocity.x, rgBody.velocity.z);
        newVelocityY = rgBody.velocity.y;

        if (newVelocityXZ.magnitude > _statistics.maxSpeed)
            newVelocityXZ = Vector3.ClampMagnitude(newVelocityXZ, _statistics.maxSpeed);

        if (newVelocityY < -10)
            newVelocityY = -10;

        rgBody.velocity = new(newVelocityXZ.x, rgBody.velocity.y, newVelocityXZ.y);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            PortalController.instance.TeleportToPosition(lastGroundedPosition);
            ResetMovement();
        }

        if (isStopped || GameController.isPaused)
            return;

        //Make movement depend on direction player is facing
        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        Vector3 rotatedMovement = transform.TransformDirection(move);

        //Move player
        rgBody.AddForce(rotatedMovement * _statistics.speedForce, ForceMode.Acceleration);
    }

    private void LateUpdate()
    {
        if (grounded)
            lastGroundedPosition = transform.position;
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if (isStopped || GameController.isPaused)
        {
            ResetMovement();
            return;
        }

        //Flipping player to direction they are going
        if (inputValue.x < 0 && !isFlipped)
        {
            transform.GetChild(0).localScale = new(-1, 1, 1);
            isFlipped = true;
        }
        else if (inputValue.x > 0 && isFlipped)
        {
            transform.GetChild(0).localScale = new(1, 1, 1);
            isFlipped = false;
        }

        movement = new Vector2(inputValue.x, inputValue.y);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (isStopped || GameController.isPaused)
            return;

        if (context.performed)
        {
            if (grounded)
            {
                Jump();
                ResetJumps();
            }
            else if (CheckIfCanJump())
                Jump();
        }
    }

    private void Jump()
    {
        rgBody.velocity = new Vector3(rgBody.velocity.x, 0f, rgBody.velocity.z);
        rgBody.AddForce(transform.up * _statistics.jumpForce, ForceMode.Impulse);
    }

    private bool CheckIfCanJump()
    {
        if (_statistics.additionalJumps.Count == 0)
            return false;

        for (int i = 0; i < _statistics.additionalJumps.Count; i++)
        {
            if (!_statistics.additionalJumps[i])
            {
                _statistics.additionalJumps[i] = true;
                return true;
            }
        }

        return false;
    }

    private void ResetJumps()
    {
        if (_statistics.additionalJumps.Count == 0)
            return;

        for (int i = 0; i < _statistics.additionalJumps.Count; i++)
            _statistics.additionalJumps[i] = false;
    }

    public void ResetMovement()
    {
        movement = Vector2.zero;
        rgBody.velocity = Vector3.zero;
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
