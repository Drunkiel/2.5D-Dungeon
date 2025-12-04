using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerController : MonoBehaviour
{
    [SerializeField] private List<string> objectTags;
    public bool canBeShown = true;

    public UnityEvent enterEvent;
    public UnityEvent stayEvent;
    public UnityEvent exitEvent;

    private void Awake()
    {
        if (TryGetComponent(out TriggerController _controller))
            SetTag(_controller.objectsTag);
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
        if (objectTags.Contains(collider.tag))
        {
            events.Invoke();
            return;
        }
    }

    public void SetTag(List<string> tags)
    {
        objectTags = tags;
    }

    public void Deactivate()
    {
        canBeShown = false;
    }
}
