using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speedForce;
    private readonly float maxSpeed = 1.2f;
    public float jumpForce;

    public float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] private bool grounded;
    [SerializeField] private bool flipped;
    public List<bool> additionalJumps = new();

    private Vector2 movement;
    private Vector2 newVelocityXZ;
    private float newVelocityY;
    public bool isMoving;

    [SerializeField] private Rigidbody rgBody;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem particle;

    private AudioSource moveSoundSource;

    // Start is called before the first frame update
    void Start()
    {
        moveSoundSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (newVelocityXZ.magnitude > maxSpeed)
            newVelocityXZ = Vector3.ClampMagnitude(newVelocityXZ, maxSpeed);

        if (newVelocityY < -10)
            newVelocityY = -10;

        rgBody.velocity = new Vector3(newVelocityXZ.x, rgBody.velocity.y, newVelocityXZ.y);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
            transform.position = new(0, 0, 0);

        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        rgBody.AddForce(move * speedForce, ForceMode.Acceleration);
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

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
        rgBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private bool CheckIfCanJump()
    {
        if (additionalJumps.Count == 0)
            return false;

        for (int i = 0; i < additionalJumps.Count; i++)
        {
            if (!additionalJumps[i])
            {
                additionalJumps[i] = true;
                return true;
            }
        }

        return false;
    }

    private void ResetJumps()
    {
        if (additionalJumps.Count == 0)
            return;

        for (int i = 0; i < additionalJumps.Count; i++)
            additionalJumps[i] = false;
    }
}
