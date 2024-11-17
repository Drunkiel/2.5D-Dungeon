using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    public float speed;
    public Vector3Int axisToRotate;

    // Update is called once per frame
    void Update()
    {
        transform.localRotation *= Quaternion.Euler(axisToRotate.x * speed, axisToRotate.y * speed, axisToRotate.z * speed);
    }
}
