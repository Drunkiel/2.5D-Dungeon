using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public bool isTriggered;
    public bool reverseReturn;
    public List<string> objectsTag = new();

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
        if (objectsTag.Contains(collider.tag))
        {
            isTriggered = reverseReturn ? !enter : enter;
            return;
        }
    }
}