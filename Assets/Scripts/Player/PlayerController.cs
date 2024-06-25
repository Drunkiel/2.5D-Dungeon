using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerStatistics _statistics;

    public bool isPlayerStopped;

    public float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] private bool grounded;
    private bool flipped;
    
    private Vector3 lastGroundedPosition;

    private Vector2 movement;
    private Vector2 newVelocityXZ;
    private float newVelocityY;
    public bool isMoving;

    public HoldingController _holdingController;
    [SerializeField] private Rigidbody rgBody;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem particle;

    private AudioSource moveSoundSource;

    // Start is called before the first frame update
    void Start()
    {
        moveSoundSource = GetComponent<AudioSource>();
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerStopped) 
            return;

        //Movement, jump and animations control
        isMoving = movement.magnitude > 0.1f;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight, whatIsGround);

        //anim.SetFloat("Movement", movement.magnitude);
        /*
                if (isMoving)
                {
                    particle.Play();
                }*/

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
            transform.position = lastGroundedPosition;

        if (isPlayerStopped)
            return;

        if (grounded)
            lastGroundedPosition = transform.position;

        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        rgBody.AddForce(move * _statistics.speedForce, ForceMode.Acceleration);
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if (isPlayerStopped)
            return;

        //Flipping player to direction they are going
        if (inputValue.x < 0 && !flipped)
        {
            transform.GetChild(0).localScale = new(-1, 1, 1);
            flipped = true;
        }
        else if (inputValue.x > 0 && flipped)
        {
            transform.GetChild(0).localScale = new(1, 1, 1);
            flipped = false;
        }

        movement = new Vector2(inputValue.x, inputValue.y);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
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
        movement = new();
    }
}
