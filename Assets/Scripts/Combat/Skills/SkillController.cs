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
    //Damage
    MeleeDamage,
    RangeDamage,
    MagicDamage,

    //Protection
    AllProtection,
    MeleeProtection,
    RangeProtection,
    MagicProtection,

    //Mana
    ManaUsage,
}

[System.Serializable]
public class Attributes
{
    public AttributeTypes attributeType;
    public ElementalTypes elementalTypes;
    public int amount;
}

public class SkillController : MonoBehaviour
{
	public SkillHolder _skillHolder;
}
