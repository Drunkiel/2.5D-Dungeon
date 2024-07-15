using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStatistics
{
    [Header("Health / Mana")]
    public int health;
    public int maxHealth;
    public float healthRegeneration;
    public int mana;
    public int maxMana;
    public float manaRegeneration;
    
    [Header("Multiplier's")]
    public float damageMultiplier = 1;
    public float allProtectionMultiplier = 1;
    public float elementalProtectionMultiplier = 1;
    public float manaUsageMultiplier = 1;

    [Header("Damage")]
    public int meleeDamage;
    public int rangeDamage;
    public int magicDamage;

    [Header("Protection")]
    public int allProtection;
    public int meleeProtection;
    public int rangeProtection;
    public int magicProtection;

    [Header("Elemental Protection")]
    public int fireResistance;
    public int waterResistance;
    public int earthResistance;
    public int airResistance;

    [Header("Movement")]
    public float speedForce;
    public float maxSpeed = 1.2f;

    public float jumpForce;
    public List<bool> additionalJumps = new();

    public void TakeDamage(float amount, AttributeTypes attributeTypes, ElementalTypes elementalTypes)
    {
        int damageToDeal = CalculateDamage(amount * damageMultiplier, attributeTypes, elementalTypes);

        health -= damageToDeal;
        if (health < 0)
        {
            health = 0;
            Debug.Log("Enemy died ;<");
        }
    }

    public int CalculateDamage(float amount, AttributeTypes attributeTypes, ElementalTypes elementalTypes)
    {
        float damageOutput = amount;

        damageOutput -= allProtection * (1.5f * allProtectionMultiplier);

        switch(attributeTypes)
        {
            case AttributeTypes.MeleeDamage:
                damageOutput -= meleeProtection * (1.2f * allProtectionMultiplier);
                break;

            case AttributeTypes.RangeDamage:
                damageOutput -= rangeProtection * (1.2f * allProtectionMultiplier);
                break;

            case AttributeTypes.MagicDamage:
                damageOutput -= magicProtection * (1.2f * allProtectionMultiplier);
                break;
        }

        switch (elementalTypes)
        {
            case ElementalTypes.NoElement:
                damageOutput -= 0;
                break;

            case ElementalTypes.Fire:
                damageOutput -= fireResistance * (2 * elementalProtectionMultiplier);
                break;

            case ElementalTypes.Water:
                damageOutput -= waterResistance * (2 * elementalProtectionMultiplier);
                break;

            case ElementalTypes.Earth:
                damageOutput -= earthResistance * (2 * elementalProtectionMultiplier);
                break;

            case ElementalTypes.Air:
                damageOutput -= airResistance * (1.25f * elementalProtectionMultiplier);
                break;
        }

        if (damageOutput <= 0)
            damageOutput = 1;

        return Mathf.FloorToInt(damageOutput);
    }

    public void TakeMana(int amount)
    {
        mana -= CalculateManaUsage(amount * manaUsageMultiplier);
        if (mana < 0)
        {
            mana = 0;
            Debug.Log("Player has no more mana ;<");
        }
    }

    private int CalculateManaUsage(float amount)
    {
        float manaOutput = amount;

        return Mathf.FloorToInt(manaOutput);
    }

    public void ResetStatistics()
    {
        maxHealth = 100;

        healthRegeneration = 1;
        manaRegeneration = 1;

        damageMultiplier = 1;
        allProtectionMultiplier = 1;
        elementalProtectionMultiplier = 1;
        manaUsageMultiplier = 1;

        meleeDamage = 0;
        rangeDamage = 0;
        magicDamage = 0;
        allProtection = 0;
        meleeProtection = 0;
        rangeProtection = 0;
        magicProtection = 0;

        speedForce = 30;
    }

    public void RecalculateStatistics(GearHolder _gearHolder)
    {
        ResetStatistics();

        //Getting weapons
        WeaponItem _weaponLeft = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Left_Hand);
        WeaponItem _weaponRight = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Right_Hand);
        WeaponItem _weaponBoth = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Both_Hands);

        //Getting armor
        ArmorItem _armorHead = _gearHolder.GetHoldingArmor(ArmorType.Helmet);
        ArmorItem _armorChestplate = _gearHolder.GetHoldingArmor(ArmorType.Chestplate);
        ArmorItem _armorBoots = _gearHolder.GetHoldingArmor(ArmorType.Boots);

        List<Attributes> _allAttributes = new();
        List<ItemBuff> _allBuffs = new();

        //Checks if any variable is empty
        if (_weaponLeft != null)
        {
            _allAttributes.AddRange(_weaponLeft._itemData._attributes);
            _allBuffs.AddRange(_weaponLeft._itemData._itemBuffs);
        }

        if (_weaponRight != null)
        {
            _allAttributes.AddRange(_weaponRight._itemData._attributes);
            _allBuffs.AddRange(_weaponRight._itemData._itemBuffs);
        }

        if (_weaponBoth != null)
        {
            _allAttributes.AddRange(_weaponBoth._itemData._attributes);
            _allBuffs.AddRange(_weaponBoth._itemData._itemBuffs);
        }

        if (_armorHead != null)
        {
            _allAttributes.AddRange(_armorHead._itemData._attributes);
            _allBuffs.AddRange(_armorHead._itemData._itemBuffs);
        }

        if (_armorChestplate != null)
        {
            _allAttributes.AddRange(_armorChestplate._itemData._attributes);
            _allBuffs.AddRange(_armorChestplate._itemData._itemBuffs);
        }

        if (_armorBoots != null)
        {
            _allAttributes.AddRange(_armorBoots._itemData._attributes);
            _allBuffs.AddRange(_armorBoots._itemData._itemBuffs);
        }

        //Setting all data
        for (int i = 0; i < _allAttributes.Count; i++)
        {
            switch (_allAttributes[i].attributeType)
            {
                case AttributeTypes.MeleeDamage:
                    meleeDamage += _allAttributes[i].amount;
                    break;

                case AttributeTypes.RangeDamage:
                    rangeDamage += _allAttributes[i].amount;
                    break;

                case AttributeTypes.MagicDamage:
                    magicDamage += _allAttributes[i].amount;
                    break;

                case AttributeTypes.AllProtection:
                    allProtection += _allAttributes[i].amount;
                    break;

                case AttributeTypes.MeleeProtection:
                    meleeProtection += _allAttributes[i].amount;
                    break;

                case AttributeTypes.RangeProtection:
                    rangeProtection += _allAttributes[i].amount;
                    break;

                case AttributeTypes.MagicProtection:
                    magicProtection += _allAttributes[i].amount;
                    break;
            }
        }

        for (int i = 0; i < _allBuffs.Count; i++)
        {
            switch(_allBuffs[i].itemBuffs)
            {
                case ItemBuffs.Damage:
                    damageMultiplier += _allBuffs[i].amount;
                    break;

                case ItemBuffs.AllProtection:
                    allProtectionMultiplier += _allBuffs[i].amount;
                    break;

                case ItemBuffs.ElementalProtection:
                    elementalProtectionMultiplier += _allBuffs[i].amount;
                    break;

                case ItemBuffs.MaxHealth:
                    maxHealth += Mathf.FloorToInt(_allBuffs[i].amount);
                    break;

                case ItemBuffs.HealthRegeneration:
                    healthRegeneration += _allBuffs[i].amount;
                    break;

                case ItemBuffs.MaxMana:
                    maxMana += Mathf.FloorToInt(_allBuffs[i].amount);
                    break;

                case ItemBuffs.ManaRegeneration:
                    manaRegeneration += _allBuffs[i].amount;
                    break;

                case ItemBuffs.ManaUsage:
                    manaUsageMultiplier += _allBuffs[i].amount;
                    break;

                case ItemBuffs.Speed:
                    speedForce += _allBuffs[i].amount;
                    break;
            }
        }
    }
}
