using UnityEngine;
using UnityEngine.UI;

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

    public void SetUpSkill(SkillDataParser _skillDataParser, int slotIndex)
    {
        _skillHolder._skillDatas.Add(_skillDataParser);

        if (_skillHolder._skillDatas[^1] == null)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"There is no skill named: {_skillDataParser._skillData.displayedName}");
            return;
        }

        CollisionController _collisionController = Instantiate(_skillHolder._skillDatas[^1].skillPrefab, skillsParent).GetComponent<CollisionController>();
        if (_combatUI._skillInfos.Count == _skillHolder.skillsID.Count)
            _combatUI._skillInfos[^1]._collisionController = _collisionController;
        else
        {
            Button skillButton = InventoryController.instance._skillSlots[slotIndex]._itemID.GetComponent<Button>();
            _combatUI._skillInfos.Add(new() { skillButton = skillButton, canBeCasted = true, _collisionController = _collisionController });
        }

        _collisionController.Configure(
            GetComponent<EntityController>()._entityInfo.entity == EntityAttitude.Friendly,
            _skillHolder._skillDatas[^1]._skillData
        );

        if (_combatUI._skillInfos[^1].skillButton != null)
            _combatUI.SetSkillToBTN(_combatUI._skillInfos.Count - 1, _skillHolder._skillDatas[^1]);
    }

    public void CastSkill(int index)
    {
        if (_combatUI._skillInfos[index] == null)
            return;

        if (_combatUI._skillInfos[index].canBeCasted)
            StartCoroutine(_combatUI.Cast(index, _skillHolder._skillDatas[index]));
    }
}
