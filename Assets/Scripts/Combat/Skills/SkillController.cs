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
    Attack,
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
            _skillHolder._skillDatas[i] = SkillContainer.instance.GetSkillByName(_skillHolder.skillNames[i]);
            
            if (_skillHolder._skillDatas[i] == null)
            {
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no skill named: {_skillHolder.skillNames[i]}");
                continue;
            }

            Instantiate(_skillHolder._skillDatas[i].skillPrefab, skillsParent);

            skillsParent.GetChild(i).GetComponent<CollisionController>().Configure(
                TryGetComponent(out PlayerController _) || GetComponent<EntityController>()._entityInfo.entity == Entity.Friendly,
                _skillHolder._skillDatas[i]._skillData
            );
        }
    }

    public void CastSkill(int index)
    {
        print($"{index}, {_combatUI._skillInfos.Count}");
        if (_combatUI._skillInfos[index] == null)
        {
            print('a');
            return;
        }

        if (_combatUI._skillInfos[index].canBeCasted)
            StartCoroutine(_combatUI.Cast(index, _skillHolder._skillDatas[index]));
    }
}
