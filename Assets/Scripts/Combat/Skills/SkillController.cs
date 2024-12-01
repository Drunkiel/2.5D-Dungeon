using UnityEngine;

public enum ElementalTypes
{
    NoElement,
    Fire,
    Water,
    Earth,
    Air,
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

    void Start()
    {
        for (int i = 0; i < _skillHolder.skillNames.Count; i++)
            _skillHolder._skillDatas[i] = SkillContainer.instance.GetSkillByName(_skillHolder.skillNames[i]);
    }

    public void CastSkill(int index)
    {
        if (_combatUI.skillInfos[index].canBeCasted)
            StartCoroutine(_combatUI.Cast(index, _skillHolder._skillDatas[index]));
    }
}
