using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public int currentCamera;
    public CinemachineVirtualCamera[] virtualCameras;
    public Transform targetObject;
    public List<Material> skyboxes = new();

    // Rotate speed
    public float rotationXSpeed = 50f;
    public float rotationYSpeed = 50f;
    public float smoothingFactor = 5f;

    // Distance from camera
    [SerializeField] private float minDistance = 0.8f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private float distanceFromTarget = 2f;
    [SerializeField] private float zoomSpeed = 2f;

    private float currentXRotation = 0f;
    private float currentYRotation = 30f;
    private float targetXRotation = 0f;
    private float targetYRotation = 30f;

    private bool rightClick;
    private Vector2 cameraRotation;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        HandleZoom();
        SetCamera();
        SmoothRotateCamera();
    }

    public void RightClick(InputAction.CallbackContext context)
    {
        if (GameController.isPC)
        {
            if (context.started)
                rightClick = true;

            if (context.canceled)
            {
                rightClick = false;
                cameraRotation = Vector2.zero;
            }
        }
        else
        {
            if (context.performed)
                rightClick = !rightClick;
        }
    }

    public void HandleRotation(InputAction.CallbackContext context)
    {
        if (!rightClick)
            return;

        Vector2 inputRotation = context.ReadValue<Vector2>();

        // NIE mnożymy przez Time.deltaTime
        targetXRotation += inputRotation.x * rotationXSpeed;
        targetYRotation -= inputRotation.y * rotationYSpeed;
        targetYRotation = Mathf.Clamp(targetYRotation, 15, 45);
    }

    private void SmoothRotateCamera()
    {
        // tu dopiero stosujemy Time.deltaTime – to jest UPDATE, nie input callback
        currentXRotation = Mathf.Lerp(currentXRotation, targetXRotation, smoothingFactor * Time.deltaTime);
        currentYRotation = Mathf.Lerp(currentYRotation, targetYRotation, smoothingFactor * Time.deltaTime);
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distanceFromTarget -= scrollInput * zoomSpeed;
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, minDistance, maxDistance);
    }

    private void SetCamera()
    {
        Vector3 direction = new(0, 0, -distanceFromTarget);
        Vector3 cameraPosition = Quaternion.Euler(currentYRotation, currentXRotation, 0) * direction + targetObject.position;

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

    public void ChangeSkyBox(int index)
    {
        if (index < 0 || index >= skyboxes.Count)
            return;

        RenderSettings.skybox = skyboxes[index];
    }
}
