using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] private bool grounded;
    [SerializeField] private Vector3 lastGroundedPosition;

    private Vector2 movement;
    private Vector2 newVelocityXZ;
    private float newVelocityY;
    public bool isMoving;

    private EntityController _controller;

    void Start()
    {
        _controller = GetComponent<EntityController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if player is moving and touching ground
        isMoving = movement.magnitude > 0.1f;
        grounded = Physics.Raycast(transform.position + new Vector3(0.05f, 0), Vector3.down, playerHeight, whatIsGround) ||
            Physics.Raycast(transform.position + new Vector3(-0.05f, 0), Vector3.down, playerHeight, whatIsGround);

        _controller.anim.SetFloat("Movement", _controller.isStopped ? 0 : movement.magnitude);

        if (_controller.isStopped || GameController.isPaused)
            return;

        // if (isMoving)
        // {
        //     particle.Play();
        // }

        //Clamping movement speed
        newVelocityXZ = new(_controller.rgBody.velocity.x, _controller.rgBody.velocity.z);
        newVelocityY = _controller.rgBody.velocity.y;

        if (newVelocityXZ.magnitude > _controller._statistics.maxSpeed)
            newVelocityXZ = Vector3.ClampMagnitude(newVelocityXZ, _controller._statistics.maxSpeed);

        if (newVelocityY < -10)
            newVelocityY = -10;

        _controller.rgBody.velocity = new(newVelocityXZ.x, _controller.rgBody.velocity.y, newVelocityXZ.y);
    }

    private void FixedUpdate()
    {
        if (transform.position.y < -10f)
        {
            PortalController.instance.TeleportToPosition(lastGroundedPosition);
            ResetMovement();
        }

        if (_controller.isStopped || GameController.isPaused)
            return;

        //Make movement depend on direction player is facing
        Vector3 move = new Vector3(movement.x, 0, movement.y).normalized;
        Vector3 rotatedMovement = transform.TransformDirection(move);

        //Move player
        _controller.rgBody.AddForce(rotatedMovement * _controller._statistics.speedForce, ForceMode.Acceleration);
    }

    private void LateUpdate()
    {
        if (grounded)
            lastGroundedPosition = transform.position;
    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        if (_controller.isStopped || GameController.isPaused)
        {
            ResetMovement();
            return;
        }

        if (inputValue.y < 0 && !_controller.isFacingCamera)
        {
            _controller.FaceCamera(true);
            GetComponent<EntityLookController>().UpdateEntityLookAll(_controller.isFacingCamera);
            GetComponent<EntityLookController>().RotateCharacter(!_controller.isFlipped, _controller.isFacingCamera);
        }
        else if (inputValue.y > 0 && _controller.isFacingCamera)
        {
            _controller.FaceCamera(false);
            GetComponent<EntityLookController>().UpdateEntityLookAll(_controller.isFacingCamera);
            GetComponent<EntityLookController>().RotateCharacter(!_controller.isFlipped, _controller.isFacingCamera);
        }

        //Flipping player to direction they are going
        if (inputValue.x < 0 && !_controller.isFlipped)
        {
            transform.GetChild(0).localScale = new(-1, 1, 1);
            _controller.Flip(true);
            GetComponent<EntityLookController>().RotateCharacter(!_controller.isFlipped, _controller.isFacingCamera);
        }
        else if (inputValue.x > 0 && _controller.isFlipped)
        {
            transform.GetChild(0).localScale = new(1, 1, 1);
            _controller.Flip(false);
            GetComponent<EntityLookController>().RotateCharacter(!_controller.isFlipped, _controller.isFacingCamera);
        }

        movement = new Vector2(inputValue.x, inputValue.y);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (_controller.isStopped || GameController.isPaused)
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
        _controller.rgBody.velocity = new Vector3(_controller.rgBody.velocity.x, 0f, _controller.rgBody.velocity.z);
        _controller.rgBody.AddForce(transform.up * _controller._statistics.jumpForce, ForceMode.Impulse);
    }

    private bool CheckIfCanJump()
    {
        if (_controller._statistics.additionalJumps.Count == 0)
            return false;

        for (int i = 0; i < _controller._statistics.additionalJumps.Count; i++)
        {
            if (!_controller._statistics.additionalJumps[i])
            {
                _controller._statistics.additionalJumps[i] = true;
                return true;
            }
        }

        return false;
    }

    private void ResetJumps()
    {
        if (_controller._statistics.additionalJumps.Count == 0)
            return;

        for (int i = 0; i < _controller._statistics.additionalJumps.Count; i++)
            _controller._statistics.additionalJumps[i] = false;
    }

    public void ResetMovement()
    {
        movement = Vector2.zero;
        _controller.rgBody.velocity = Vector3.zero;
    }
}
