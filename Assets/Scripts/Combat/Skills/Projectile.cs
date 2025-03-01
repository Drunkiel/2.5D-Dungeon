using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public float force;
    public bool useGravity;
    public UnityEvent unityEvent;

    public void Shoot()
    {
        GameObject skillCopyObject = Instantiate(transform.GetChild(0).gameObject, transform.position, transform.rotation, null);
        Rigidbody skillCopyRgBody = skillCopyObject.GetComponent<Rigidbody>();
        skillCopyObject.transform.GetChild(0).gameObject.SetActive(true);
        skillCopyObject.transform.GetComponent<EventTriggerController>().enterEvent = unityEvent;
        skillCopyRgBody.useGravity = useGravity;
        skillCopyRgBody.constraints = RigidbodyConstraints.FreezeRotation;
        skillCopyRgBody.AddForce(Vector3.forward * force);
    }
}
