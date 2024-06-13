using UnityEngine;

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
    public int amount;
}

public class ItemID : MonoBehaviour
{
    public ItemType itemType;
    public WeaponItem _weaponItem;
    public ArmorItem _armorItem;
    public CollectableItem _collectableItem;
}
