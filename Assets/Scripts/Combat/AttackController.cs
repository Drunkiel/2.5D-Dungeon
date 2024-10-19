using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public string entityTag;
    public List<GameObject> targets = new();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(entityTag) && !other.isTrigger)
            targets.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(entityTag))
            targets.Remove(other.gameObject);
    }
}
