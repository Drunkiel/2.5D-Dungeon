using UnityEngine;

public enum ElementalTypes
{
    NoElement,
    Divine,
    Demonic,
}

public enum AttributeTypes
{
    Buff,

    //Damage
    MeleeDamage,
    RangeDamage,
    MagicDamage,

    //Protection
    AllProtection,
    MeleeProtection,
    RangeProtection,
    MagicProtection,

    //Cooldown
    Cooldown,

    //Mana
    ManaUsage,
}

public enum SkillType
{
    AttackMelee,
    AttackRange,
    Defence,
}

[System.Serializable]
public class Attributes
{
    public AttributeTypes attributeType;
    public ElementalTypes elementalTypes;
    public Buffs buffTypes;
    public int amount;
}

public class SkillController : MonoBehaviour
{
    public SkillHolder _skillHolder;
    public CombatUI _combatUI;
    public Transform skillsParent;

    void Awake()
    {
        for (int i = 0; i < _skillHolder.skillNames.Count; i++)
        {
            _skillHolder._skillDatas.Add(SkillContainer.instance.GetSkillByName(_skillHolder.skillNames[i]));

            if (_skillHolder._skillDatas[i] == null)
            {
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no skill named: {_skillHolder.skillNames[i]}");
                continue;
            }

            CollisionController _collisionController = Instantiate(_skillHolder._skillDatas[i].skillPrefab, skillsParent).GetComponent<CollisionController>();
            if (_combatUI._skillInfos.Count == _skillHolder.skillNames.Count)
                _combatUI._skillInfos[i]._collisionController = _collisionController;
            else
                _combatUI._skillInfos.Add(new() { canBeCasted = true, _collisionController = _collisionController });

            _collisionController.Configure(
                GetComponent<EntityController>()._entityInfo.entity == EntityAttitude.Friendly,
                _skillHolder._skillDatas[i]._skillData
            );
        }
    }

    public void CastSkill(int index)
    {
        if (_combatUI._skillInfos[index] == null)
            return;

        if (_combatUI._skillInfos[index].canBeCasted)
            StartCoroutine(_combatUI.Cast(index, _skillHolder._skillDatas[index]));
    }
}
