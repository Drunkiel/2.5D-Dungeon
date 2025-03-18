using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public List<GameObject> targets = new();

    public string[] entityTags;

    public void Configure(bool isPlayer, SkillData _skillData)
    {
        if (!_skillData.worksOnSelf && !_skillData.worksOnOthers)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Collision controller must detect something", OutputType.Error);
            return;
        }

        //Resize collider and set center
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = _skillData.size;
        boxCollider.center = _skillData.center;

        //Set what tag to check
        if (_skillData.worksOnOthers && !_skillData.worksOnSelf)
            entityTags = isPlayer ? new string[] { "Enemy" } : new string[] { "Player", "Friend" };
        else if (_skillData.worksOnOthers && _skillData.worksOnSelf)
            entityTags = isPlayer ? new string[] { "Player", "Friend" } : new string[] { "Enemy" };

        if (_skillData.worksOnSelf)
            targets.Add(transform.parent.parent.parent.gameObject);
    }

    void OnTriggerEnter(Collider collider)
    {
        CheckCollision(collider, true);
    }

    void OnTriggerExit(Collider collider)
    {
        CheckCollision(collider, false);
    }

    void CheckCollision(Collider collider, bool addTarget)
    {
        foreach (string entityTag in entityTags)
        {
            if (collider.CompareTag(entityTag))
            {
                if (addTarget && !targets.Contains(collider.gameObject))
                    targets.Add(collider.gameObject);
                else if (!addTarget && targets.Contains(collider.gameObject))
                    targets.Remove(collider.gameObject);
            }
        }
    }
}
