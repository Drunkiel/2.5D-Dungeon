using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public float force;
    public bool useGravity;
    public UnityEvent unityEvent;
    public CollisionController _collisionController;
    public EventTriggerController _triggerController;

    public void Shoot()
    {
        GameObject skillCopyObject = Instantiate(gameObject, transform.position, transform.rotation, null);
        Rigidbody skillCopyRgBody = skillCopyObject.GetComponent<Rigidbody>();
        skillCopyObject.transform.GetChild(0).gameObject.SetActive(true);
        skillCopyRgBody.useGravity = useGravity;
        skillCopyRgBody.constraints = RigidbodyConstraints.FreezeRotation;
        skillCopyRgBody.AddForce(Vector3.forward * force);
        _triggerController.stayEvent = unityEvent;
    }
}
