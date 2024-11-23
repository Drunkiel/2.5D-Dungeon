using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuilderCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public Transform target;           // Target around which the camera will rotate
    public float rotationSpeed = 100f; // Speed of camera rotation
    public float zoomSpeed = 10f;      // Speed of zoom in/out
    public float minZoom = 20f;        // Minimum Field of View (zoom in limit)
    public float maxZoom = 60f;        // Maximum Field of View (zoom out limit)
    public float distanceFromTarget = 10f; // Initial distance from the target

    private float currentRotationX = 0f;
    private float currentRotationY = 0f;

    // Limits for the vertical rotation (X axis), preventing the camera from going below the target
    public float minVerticalAngle = -10f; // Min angle, prevent the camera from going below the target
    public float maxVerticalAngle = 80f;  // Max angle, prevents too much upward rotation

    [SerializeField] private Vector2 movement;
    private Vector2 newVelocityXZ;
    public bool isMoving;

    [SerializeField] private Rigidbody rgBody;
    public float movementDamping = 5f;   // Damping for velocity reduction when not moving

    private void Start()
    {
        // Set initial position based on the target's position
        if (target != null)
            HandleRotation();
    }

    private void Update()
    {
        // Check if the right mouse button is held down for rotation
        if (Input.GetMouseButton(1))
            HandleRotation();
        HandleZoom();

        // Movement, jump and animations control
        isMoving = movement.magnitude > 0.1f;

        // If not moving, dampen the velocity
        if (!isMoving)
            // Smoothly reduce velocity to zero when no movement input is provided
            rgBody.velocity = Vector3.Lerp(rgBody.velocity, Vector3.zero, Time.deltaTime * movementDamping);

        // Clamping movement speed
        newVelocityXZ = new Vector2(rgBody.velocity.x, rgBody.velocity.z);

        if (newVelocityXZ.magnitude > 1.5f)
            newVelocityXZ = Vector2.ClampMagnitude(newVelocityXZ, 1.5f);

        rgBody.velocity = new Vector3(newVelocityXZ.x, 0, newVelocityXZ.y);
    }

private void FixedUpdate()
{
    if (target != null)
    {
        // Ustaw pozycjê celu w p³aszczyŸnie XZ
        target.localPosition = new Vector3(target.position.x, 0, target.position.z);

        // SprawdŸ, czy pozycja celu przekracza granice mapy
        if (Mathf.Abs(target.position.x) > BuildingSystem.instance.mapSize.x ||
            Mathf.Abs(target.position.z) > BuildingSystem.instance.mapSize.y)
        {
            // Oblicz wektor powrotu do punktu (0, 0, 0)
            Vector3 returnDirection = (Vector3.zero - target.position).normalized;

            // Wymuœ ruch gracza w stronê (0, 0, 0)
            rgBody.AddForce(returnDirection * 50, ForceMode.Impulse);

            return;
        }

        // Oblicz kierunek ruchu na podstawie orientacji kamery
        Vector3 moveDirection = (movement.x * target.right + movement.y * target.forward).normalized;

        // Zastosuj si³ê w kierunku ruchu
        rgBody.AddForce(moveDirection * 10, ForceMode.Acceleration);
    }
}


    public void MovementInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();

        movement = new Vector2(inputValue.x, inputValue.y);
    }

    //Handle rotation and rotate target to match the camera
    private void HandleRotation()
    {
        //Get mouse input for horizontal and vertical movement
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Update the current rotations based on mouse movement
        currentRotationY += mouseX * rotationSpeed * Time.deltaTime;
        currentRotationX -= mouseY * rotationSpeed * Time.deltaTime;

        //Clamp vertical rotation to avoid flipping the camera over and going below the object
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);

        //Update the camera position based on the new rotation
        if (target != null)
        {
            // Rotate around the target while maintaining distance
            Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
            Vector3 direction = rotation * Vector3.back * distanceFromTarget;
            virtualCamera.transform.position = target.position + direction;

            target.rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);
        }
    }

    private void HandleZoom()
    {
        //Get the scroll wheel input (positive for zooming in, negative for zooming out)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        //Update the distance from the target based on the scroll input
        distanceFromTarget -= scrollInput * zoomSpeed;

        //Clamp the distance to avoid zooming too close or too far
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, minZoom, maxZoom);
        virtualCamera.m_Lens.FieldOfView = distanceFromTarget;
    }
}