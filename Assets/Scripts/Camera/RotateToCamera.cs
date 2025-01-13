using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private float rotationDistanceThreshold = 5f;

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
        directionToCamera.y = 0f;

        //Rotate towards camera
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            targetRotation *= Quaternion.Euler(0, 180f, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1.5f);
        }
    }
}
