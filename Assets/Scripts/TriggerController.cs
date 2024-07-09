using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public bool isTriggered;
    [SerializeField] private bool reverseReturn;
    public string[] objectsTag;
    public HashSet<string> objectsTagsSet;

    void Awake()
    {
        objectsTagsSet = new HashSet<string>(objectsTag);
    }

    void OnTriggerEnter(Collider collider)
    {
        CheckCollision(collider);
    }

    void OnTriggerStay(Collider collider)
    {
        CheckCollision(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        CheckCollision(collider, false);
    }

    void CheckCollision(Collider collider, bool enter = true)
    {
        if (objectsTagsSet == null)
            return;

        if (objectsTagsSet.Contains(collider.tag))
        {
            isTriggered = reverseReturn ? !enter : enter;
            return;
        }
    }
}
