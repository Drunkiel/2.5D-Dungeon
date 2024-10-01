using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private int currentCamera;
    public CinemachineVirtualCamera[] virtualCameras;
    public Transform targetObject;

    //Rotate speed
    public float rotationSpeed = 50f;

    //Distance from camera
    private float minDistance = 0.8f;
    private float maxDistance = 2f;
    private float distanceFromTarget = 1f;
    private float zoomSpeed = 2f;

    private float currentXRotation = 0f;
    [SerializeField] private float currentYRotation = 30f;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleZoom();
        SetCamera();
    }

    public void HandleRotation(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        //Get Input
        float mouseX = context.ReadValue<Vector2>().x;
        mouseX = Mathf.Clamp(mouseX, -1, 1);
        currentXRotation += mouseX * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        //Get scroll input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        //Adjust the distance based on scroll input and zoom speed
        distanceFromTarget -= scrollInput * zoomSpeed;

        //Clamp the distance to be within the min and max range
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, minDistance, maxDistance);
    }

    private void SetCamera()
    {
        //New position to camera
        Vector3 direction = new(0, 0, -distanceFromTarget);
        Vector3 cameraPosition = Quaternion.Euler(currentYRotation, currentXRotation, 0) * direction + targetObject.position;

        //Set position
        virtualCameras[currentCamera].transform.position = cameraPosition;
        targetObject.transform.rotation = Quaternion.Euler(0, currentXRotation, 0);
    }

    public void SetCamera(int index)
    {
        if (index >= virtualCameras.Length)
            return;

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (i != index)
                virtualCameras[i].Priority = 1;
            else
            {
                currentCamera = index;
                virtualCameras[index].Priority = 99;
            }
        }
    }

    public void ResetZoom()
    {
        virtualCameras[currentCamera].m_Lens.FieldOfView = 80;
    }

    public IEnumerator ZoomTo(int number, float timeToEnd)
    {
        float startValue = virtualCameras[0].m_Lens.FieldOfView;
        float time = 0f;

        while (time < timeToEnd)
        {
            time += Time.deltaTime;
            virtualCameras[currentCamera].m_Lens.FieldOfView = Mathf.Lerp(startValue, number, time / timeToEnd);
            yield return null;
        }
    }
}
