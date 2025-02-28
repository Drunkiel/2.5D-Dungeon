using UnityEngine;
using UnityEngine.Events;

public class EventTriggerController : MonoBehaviour
{
    private string objectTag;
    public bool canBeShown = true;

    public UnityEvent enterEvent;
    public UnityEvent stayEvent;
    public UnityEvent exitEvent;

    private void Awake()
    {
        if (TryGetComponent(out TriggerController _controller))
            objectTag = _controller.objectsTag[0];
    }

    void OnTriggerEnter(Collider collider)
    {
        if (canBeShown)
            CheckCollision(collider, enterEvent);
    }

    void OnTriggerStay(Collider collider)
    {
        if (canBeShown)
            CheckCollision(collider, stayEvent);
    }

    void OnTriggerExit(Collider collider)
    {
        if (canBeShown)
            CheckCollision(collider, exitEvent);
    }

    void CheckCollision(Collider collider, UnityEvent events)
    {
        if (objectTag.Contains(collider.tag))
        {
            events.Invoke();
            return;
        }
    }
}
