using UnityEngine;

public enum ItemBuffs
{
    NoBuff,
    //Damage
    Damage,
    //Protection
    AllProtection,
    ElementalProtection,
    //Health
    MaxHealth,
    HealthRegeneration,
    //Mana
    MaxMana,
    ManaRegeneration,
    ManaUsage,
    //Speed
    Speed,
}

[System.Serializable]
public class ItemBuff
{
    public ItemBuffs itemBuffs;
    public ValueType valueType;
    public float amount = 1;
}

public class ItemID : MonoBehaviour
{
    public ItemData _itemData;
    public WeaponItem _weaponItem;
    public ArmorItem _armorItem;
    public CollectableItem _collectableItem;
    public SkillDataParser _skillDataParser;
}
