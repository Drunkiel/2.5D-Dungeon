using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BasicStatistics
{
    public int maxHealth;

    public float healthRegeneration;
    public float manaRegeneration;

    public float damageMultiplier;
    public float allProtectionMultiplier;
    public float elementalProtectionMultiplier;
    public float manaUsageMultiplier;

    public int meleeDamage;
    public int rangeDamage;
    public int magicDamage;
    public int allProtection;
    public int meleeProtection;
    public int rangeProtection;
    public int magicProtection;

    public float speedForce;
    public float maxSpeed;
}

[Serializable]
public class EntityStatistics
{
    [HideInInspector] public BasicStatistics _stats;

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

    [Header("Active Buffs")]
    public List<Buff> _activeBuffs = new();
    public EntityStatsController _statsController;

    public void SaveStats()
    {
        _stats.maxHealth = maxHealth;

        _stats.healthRegeneration = healthRegeneration;
        _stats.manaRegeneration = manaRegeneration;

        _stats.damageMultiplier = damageMultiplier;
        _stats.allProtectionMultiplier = allProtectionMultiplier;
        _stats.elementalProtectionMultiplier = elementalProtectionMultiplier;
        _stats.manaUsageMultiplier = manaUsageMultiplier;

        _stats.meleeDamage = meleeDamage;
        _stats.rangeDamage = rangeDamage;
        _stats.magicDamage = magicDamage;
        _stats.allProtection = allProtection;
        _stats.meleeProtection = meleeProtection;
        _stats.rangeProtection = rangeProtection;
        _stats.magicProtection = magicProtection;

        _stats.speedForce = speedForce;
        _stats.maxSpeed = maxSpeed;
    }

    public void TakeDamage(float amount, AttributeTypes attributeTypes, ElementalTypes elementalTypes, bool ignore = false)
    {
        int damageToDeal;

        if (!ignore)
            damageToDeal = CalculateDamage(amount * damageMultiplier, attributeTypes, elementalTypes);
        else
            damageToDeal = (int)amount;

        health -= damageToDeal;
        _statsController.UpdateHealthSlider(health, maxHealth, ignore);

        if (health < 0)
        {
            health = 0;
            ConsoleController.instance.ChatMessage(SenderType.System, "Entity is dead :p");
        }

        if (health > maxHealth)
            health = maxHealth;
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

    public void TakeMana(float amount, bool ignore = false)
    {
        if (!ignore)
            mana -= CalculateManaUsage(amount * manaUsageMultiplier);
        else    
            mana -= (int)amount;

        _statsController.UpdateManaSlider(mana, maxMana, ignore);

        if (mana < 0)
            mana = 0;

        if (mana > maxMana)
            mana = maxMana;
    }

    private int CalculateManaUsage(float amount)
    {
        return Mathf.FloorToInt(amount);
    }

    public void UpdateBuffs(GearHolder _gearHolder)
    {
        bool buffExpired = false;

        // Przechodzimy przez listê Buffów
        for (int i = _activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = _activeBuffs[i];
            // Jeœli Buff siê skoñczy³, usuwamy go
            if (!buff.UpdateBuff())
            {
                _activeBuffs.RemoveAt(i);
                buffExpired = true;
            }
        }

        if (buffExpired)
            RecalculateStatistics(_gearHolder);
    }

    public void ResetStatistics()
    {
        maxHealth = _stats.maxHealth;

        healthRegeneration = _stats.healthRegeneration;
        manaRegeneration = _stats.manaRegeneration;

        damageMultiplier = _stats.damageMultiplier;
        allProtectionMultiplier = _stats.allProtectionMultiplier;
        elementalProtectionMultiplier = _stats.elementalProtectionMultiplier;
        manaUsageMultiplier = _stats.manaUsageMultiplier;

        meleeDamage = _stats.meleeDamage;
        rangeDamage = _stats.rangeDamage;
        magicDamage = _stats.magicDamage;
        allProtection = _stats.allProtection;
        meleeProtection = _stats.meleeProtection;
        rangeProtection = _stats.rangeProtection;
        magicProtection = _stats.magicProtection;

        speedForce = _stats.speedForce;
        maxSpeed = _stats.maxSpeed;
    }

    public void RecalculateStatistics(GearHolder _gearHolder)
    {
        ResetStatistics();
        _statsController.RemoveBuffImages();

        //Getting weapons
        WeaponItem _weaponItemLeft = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Left_Hand);
        WeaponItem _weaponItemRight = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Right_Hand);
        WeaponItem _weaponItemBoth = _gearHolder.GetHoldingWeapon(WeaponHoldingType.Both_Hands);

        ItemID _weaponLeft = null;
        ItemID _weaponRight = null;;
        ItemID _weaponBoth = null;;

        if (_weaponItemLeft != null)
            _weaponLeft = _weaponItemLeft.GetComponent<ItemID>();

        if (_weaponItemRight != null)
            _weaponRight = _weaponItemRight.GetComponent<ItemID>();

        if (_weaponItemBoth != null)
            _weaponBoth = _weaponItemBoth.GetComponent<ItemID>();

        //Getting armor
        ArmorItem _armorItemHead = _gearHolder.GetHoldingArmor(ArmorType.Helmet);
        ArmorItem _armorItemChestplate = _gearHolder.GetHoldingArmor(ArmorType.Chestplate);
        ArmorItem _armorItemBoots = _gearHolder.GetHoldingArmor(ArmorType.Boots);

        ItemID _armorHead = null;
        ItemID _armorChestplate = null;
        ItemID _armorBoots = null;

        if (_armorItemHead != null)
            _armorHead = _armorItemHead.GetComponent<ItemID>();

        if (_armorItemChestplate != null)
            _armorChestplate = _armorItemChestplate.GetComponent<ItemID>();

        if (_armorItemBoots != null)
            _armorBoots = _armorItemBoots.GetComponent<ItemID>();

        List<Attributes> _allAttributes = new();
        List<ItemBuff> _allItemBuffs = new();

        //Checks if any variable is empty
        if (_weaponLeft != null)
        {
            _allAttributes.AddRange(_weaponLeft._itemData._attributes);
            _allItemBuffs.AddRange(_weaponLeft._itemData._itemBuffs);
        }

        if (_weaponRight != null)
        {
            _allAttributes.AddRange(_weaponRight._itemData._attributes);
            _allItemBuffs.AddRange(_weaponRight._itemData._itemBuffs);
        }

        if (_weaponBoth != null)
        {
            _allAttributes.AddRange(_weaponBoth._itemData._attributes);
            _allItemBuffs.AddRange(_weaponBoth._itemData._itemBuffs);
        }

        if (_armorHead != null)
        {
            _allAttributes.AddRange(_armorHead._itemData._attributes);
            _allItemBuffs.AddRange(_armorHead._itemData._itemBuffs);
        }

        if (_armorChestplate != null)
        {
            _allAttributes.AddRange(_armorChestplate._itemData._attributes);
            _allItemBuffs.AddRange(_armorChestplate._itemData._itemBuffs);
        }

        if (_armorBoots != null)
        {
            _allAttributes.AddRange(_armorBoots._itemData._attributes);
            _allItemBuffs.AddRange(_armorBoots._itemData._itemBuffs);
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

        for (int i = 0; i < _allItemBuffs.Count; i++)
        {
            switch(_allItemBuffs[i].itemBuffs)
            {
                case ItemBuffs.Damage:
                    damageMultiplier += _allItemBuffs[i].amount;
                    break;

                case ItemBuffs.AllProtection:
                    allProtectionMultiplier += _allItemBuffs[i].amount;
                    break;

                case ItemBuffs.ElementalProtection:
                    elementalProtectionMultiplier += _allItemBuffs[i].amount;
                    break;

                case ItemBuffs.MaxHealth:
                    maxHealth += Mathf.FloorToInt(_allItemBuffs[i].amount);
                    break;

                case ItemBuffs.HealthRegeneration:
                    healthRegeneration += _allItemBuffs[i].amount;
                    break;

                case ItemBuffs.MaxMana:
                    maxMana += Mathf.FloorToInt(_allItemBuffs[i].amount);
                    break;

                case ItemBuffs.ManaRegeneration:
                    manaRegeneration += _allItemBuffs[i].amount;
                    break;

                case ItemBuffs.ManaUsage:
                    manaUsageMultiplier += _allItemBuffs[i].amount;
                    break;

                case ItemBuffs.Speed:
                    speedForce += _allItemBuffs[i].amount;
                    break;
            }
        }

        //Adding buffs
        for (int i = 0; i < _activeBuffs.Count; i++)
        {
            switch (_activeBuffs[i].buffType)
            {
                case Buffs.MaxHealth:
                    maxHealth *= _activeBuffs[i].buffMultiplier;
                    break;

                case Buffs.MaxMana:
                    maxMana *= _activeBuffs[i].buffMultiplier;
                    break;

                case Buffs.Damage:
                    damageMultiplier *= _activeBuffs[i].buffMultiplier;
                    break;

                case Buffs.Protection:
                    allProtectionMultiplier *= _activeBuffs[i].buffMultiplier;
                    break;

                case Buffs.MaxSpeed:
                    maxSpeed *= _activeBuffs[i].buffMultiplier;
                    break;
            }

            _statsController.AddNewBuffImage(_activeBuffs[i].sprite);
        }
    }
}
