using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public string entityTag;
    public List<GameObject> targets = new();

    public void Configure(bool isPlayer, SkillData _skillData)
    {
        if (!_skillData.worksOnSelf && !_skillData.worksOnOthers)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Collision controller is must detect something", OutputType.Error);
            return;
        }

        //Resize collider and set center
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = _skillData.size;
        boxCollider.center = _skillData.center;

        //Set what tag to check
        if (_skillData.worksOnOthers && !_skillData.worksOnSelf)
            entityTag = isPlayer ? "Enemy" : "Player";
        else if (_skillData.worksOnOthers && _skillData.worksOnSelf)
            entityTag = isPlayer ? "Player" : "Enemy";

        if (_skillData.worksOnSelf)
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
