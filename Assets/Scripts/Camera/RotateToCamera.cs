using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private CameraController _cameraController;
    private float rotationDistanceThreshold = 5f;

    private void Start()
    {
        _cameraController = CameraController.instance;
    }

    private void Update()
    {
        //Check distance between object and camera
        float distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);

        if (distanceToCamera <= rotationDistanceThreshold)
            RotateObject();
    }

    private void RotateObject()
    {
        //Calculate direction to the camera but only consider the horizontal plane (Y axis rotation)
        Vector3 directionToCamera = Camera.main.transform.position - transform.position;

        //Zero out the Y-axis in the direction vector so rotation happens only around Y
        directionToCamera.y = 0f;

        //If the direction vector has a magnitude greater than 0 (to avoid errors), calculate rotation
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            //Calculate the target rotation (only Y-axis)
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            //Add 180 degrees to the Y rotation (rotate the object by 180 degrees on Y-axis)
            targetRotation *= Quaternion.Euler(0, 180f, 0);

            //Smoothly rotate towards the target rotation on the Y-axis
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1.5f);
        }
    }
}
