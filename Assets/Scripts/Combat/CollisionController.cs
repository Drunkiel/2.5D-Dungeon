using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public string entityTag;
    public List<GameObject> targets = new();

    public void Configure(bool isPlayer, bool worksOnSelf, bool worksOnOthers)
    {
        if (!worksOnSelf && !worksOnOthers)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Collision controller is must detect something", OutputType.Error);
            return;
        }

        if (worksOnOthers && !worksOnSelf)
            entityTag = isPlayer ? "Enemy" : "Player";
        else if (worksOnOthers && worksOnSelf)
            entityTag = isPlayer ? "Player" : "Enemy";

        if (worksOnSelf)
            targets.Add(transform.parent.parent.parent.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (entityTag.Length == 0)
            return;

        if (other.CompareTag(entityTag) && !other.isTrigger)
            targets.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (entityTag.Length == 0)
            return;

        if (other.CompareTag(entityTag))
            targets.Remove(other.gameObject);
    }
}
