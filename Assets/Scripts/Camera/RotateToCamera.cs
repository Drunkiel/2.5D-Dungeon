using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private readonly float rotationDistanceThresholdSqr = 25f; //5*5
    private Transform _cameraTransform;

    private void Awake()
    {
        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null)
            return;

        Vector3 offset = _cameraTransform.position - transform.position;
        if (offset.sqrMagnitude <= rotationDistanceThresholdSqr)
            RotateObject(offset);
    }

    private void RotateObject(Vector3 directionToCamera)
    {
        directionToCamera.y = 0f;

        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            targetRotation *= Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1.5f);
        }
    }
}