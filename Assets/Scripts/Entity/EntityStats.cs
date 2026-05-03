using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EntityStatsSnapshot
{
    public float maxHealth;
    public float maxMana;

    public float meleeDamage;
    public float rangeDamage;
    public float magicDamage;

    public float allProtection;
    public float meleeProtection;
    public float rangeProtection;
    public float magicProtection;

    public float fireRes;
    public float waterRes;
    public float earthRes;
    public float airRes;

    public float speedForce;
    public float maxSpeed;
}

[System.Serializable]
public class EntityStats
{
    [Header("Core Modules")]
    public VitalStats vitals = new();
    public DamageStats damage = new();
    public ProtectionStats protection = new();
    public ElementalStats elemental = new();
    public MovementStats movement = new();

    [Header("Active Buffs")]
    public List<Buff> _activeBuffs = new();
    public EntityStatsController _statsController;

    [Header("OnDeath")]
    public UnityEvent onDeath;

    private EntityStatsSnapshot _baseSnapshot;

    public void TakeDamage(
        float amount,
        AttributeTypes attributeType,
        ElementType elementType,
        EntityStats attacker,
        bool ignore = false)
    {
        int finalDamage;

        if (!ignore)
        {
            finalDamage = DamageStats.CalculateDamage(
                amount,
                attributeType,
                elementType,
                attacker.damage,
                protection,
                elemental);
        }
        else
        {
            finalDamage = Mathf.FloorToInt(amount);
        }

        vitals.health -= finalDamage;
        vitals.health = (int)Mathf.Clamp(vitals.health, 0, vitals.maxHealth.Value);
        _statsController.UpdateHealthSlider(vitals.health, vitals.maxHealth.Value, ignore);

        if (vitals.health <= 0)
            onDeath?.Invoke();
    }

    public void TakeMana(float amount, bool ignore = false)
    {
        float finalAmount = ignore ? amount : amount * vitals.manaUsageMultiplier.Value;

        vitals.mana -= Mathf.FloorToInt(finalAmount);
        vitals.mana = Mathf.Clamp(vitals.mana, 0, Mathf.FloorToInt(vitals.maxMana.Value));
        _statsController.UpdateManaSlider(vitals.mana, vitals.maxMana.Value, ignore);
    }

    public void UpdateBuffs(GearHolder _gearHolder)
    {
        bool buffExpired = false;

        //Check if any buff expired
        for (int i = _activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = _activeBuffs[i];
            //If yes delete
            if (!buff.UpdateBuff())
            {
                _activeBuffs.RemoveAt(i);
                buffExpired = true;
            }
        }

        if (buffExpired)
            RecalculateStatistics(_gearHolder);
    }

    public void SaveStats()
    {
        _baseSnapshot = new EntityStatsSnapshot
        {
            maxHealth = vitals.maxHealth.BaseValue,
            maxMana = vitals.maxMana.BaseValue,

            meleeDamage = damage.meleeDamage.BaseValue,
            rangeDamage = damage.rangeDamage.BaseValue,
            magicDamage = damage.magicDamage.BaseValue,

            allProtection = protection.allProtection.BaseValue,
            meleeProtection = protection.meleeProtection.BaseValue,
            rangeProtection = protection.rangeProtection.BaseValue,
            magicProtection = protection.magicProtection.BaseValue,

            fireRes = elemental.fireResistance.BaseValue,
            waterRes = elemental.waterResistance.BaseValue,
            earthRes = elemental.earthResistance.BaseValue,
            airRes = elemental.airResistance.BaseValue,

            speedForce = movement.speedForce,
            maxSpeed = movement.maxSpeed
        };
    }

    private void ResetToBaseStats()
    {
        if (_baseSnapshot == null)
            return;

        vitals.maxHealth.BaseValue = _baseSnapshot.maxHealth;
        vitals.maxMana.BaseValue = _baseSnapshot.maxMana;

        damage.meleeDamage.BaseValue = _baseSnapshot.meleeDamage;
        damage.rangeDamage.BaseValue = _baseSnapshot.rangeDamage;
        damage.magicDamage.BaseValue = _baseSnapshot.magicDamage;

        protection.allProtection.BaseValue = _baseSnapshot.allProtection;
        protection.meleeProtection.BaseValue = _baseSnapshot.meleeProtection;
        protection.rangeProtection.BaseValue = _baseSnapshot.rangeProtection;
        protection.magicProtection.BaseValue = _baseSnapshot.magicProtection;

        elemental.fireResistance.BaseValue = _baseSnapshot.fireRes;
        elemental.waterResistance.BaseValue = _baseSnapshot.waterRes;
        elemental.earthResistance.BaseValue = _baseSnapshot.earthRes;
        elemental.airResistance.BaseValue = _baseSnapshot.airRes;

        movement.speedForce = _baseSnapshot.speedForce;
        movement.maxSpeed = _baseSnapshot.maxSpeed;

        //Reset values
        damage.Reset();
        protection.Reset();
        elemental.Reset();
        vitals.maxHealth.ResetModifiers();
        vitals.maxMana.ResetModifiers();
    }

    public void RecalculateStatistics(GearHolder _gearHolder)
    {
        ResetToBaseStats();
        _statsController._buffController.RemoveAllBuffs();

        //Get gear attributes and buffs
        if (_gearHolder != null)
        {
            List<Attributes> _attributes = _gearHolder.GetAllAttributes();
            List<ItemBuff> _itemBuffs = _gearHolder.GetAllBuffs();

            foreach (var attr in _attributes)
                ApplyAttribute(attr);

            foreach (var buff in _itemBuffs)
                ApplyItemBuff(buff);
        }

        //Add buffs
        foreach (var buff in _activeBuffs)
        {
            ApplyRuntimeBuff(buff);
            _statsController?._buffController.AddBuff(buff);
        }

        //Clamp values
        vitals.health = Mathf.Clamp(vitals.health, 0, (int)vitals.maxHealth.Value);
        vitals.mana = Mathf.Clamp(vitals.mana, 0, (int)vitals.maxMana.Value);
    }

    private void ApplyAttribute(Attributes attr)
    {
        switch (attr.attributeType)
        {
            case AttributeTypes.MeleeDamage:
                if (attr.valueType == ValueType.Flat)
                    damage.meleeDamage.AddFlat(attr.amount);
                else //Percentage
                    damage.meleeDamage.AddPercent(attr.amount);
                break;

            case AttributeTypes.RangeDamage:
                if (attr.valueType == ValueType.Flat)
                    damage.rangeDamage.AddFlat(attr.amount);
                else //Percentage
                    damage.rangeDamage.AddPercent(attr.amount);
                break;

            case AttributeTypes.MagicDamage:
                if (attr.valueType == ValueType.Flat)
                    damage.magicDamage.AddFlat(attr.amount);
                else //Percentage
                    damage.magicDamage.AddPercent(attr.amount);
                break;

            case AttributeTypes.AllProtection:
                if (attr.valueType == ValueType.Flat)
                    protection.allProtection.AddFlat(attr.amount);
                else //Percentage
                    protection.allProtection.AddPercent(attr.amount);
                break;

            case AttributeTypes.MeleeProtection:
                if (attr.valueType == ValueType.Flat)
                    protection.meleeProtection.AddFlat(attr.amount);
                else //Percentage
                    protection.meleeProtection.AddPercent(attr.amount);
                break;

            case AttributeTypes.RangeProtection:
                if (attr.valueType == ValueType.Flat)
                    protection.rangeProtection.AddFlat(attr.amount);
                else //Percentage
                    protection.rangeProtection.AddPercent(attr.amount);
                break;

            case AttributeTypes.MagicProtection:
                if (attr.valueType == ValueType.Flat)
                    protection.magicProtection.AddFlat(attr.amount);
                else //Percentage
                    protection.magicProtection.AddPercent(attr.amount);
                break;

            case AttributeTypes.ElementalResistance:
                if (attr.valueType == ValueType.Flat)
                    elemental.elementalProtectionMultiplier.AddFlat(attr.amount);
                else //Percentage
                    elemental.elementalProtectionMultiplier.AddPercent(attr.amount);
                break;

            case AttributeTypes.FireResistance:
                if (attr.valueType == ValueType.Flat)
                    elemental.fireResistance.AddFlat(attr.amount);
                else //Percentage
                    elemental.fireResistance.AddPercent(attr.amount);
                break;

            case AttributeTypes.WaterResistance:
                if (attr.valueType == ValueType.Flat)
                    elemental.waterResistance.AddFlat(attr.amount);
                else //Percentage
                    elemental.waterResistance.AddPercent(attr.amount);
                break;

            case AttributeTypes.EarthResistance:
                if (attr.valueType == ValueType.Flat)
                    elemental.earthResistance.AddFlat(attr.amount);
                else //Percentage
                    elemental.earthResistance.AddPercent(attr.amount);
                break;

            case AttributeTypes.AirResistance:
                if (attr.valueType == ValueType.Flat)
                    elemental.airResistance.AddFlat(attr.amount);
                else //Percentage
                    elemental.airResistance.AddPercent(attr.amount);
                break;
        }
    }

    private void ApplyItemBuff(ItemBuff buff)
    {
        switch (buff.itemBuffs)
        {
            case ItemBuffs.Damage:
                if (buff.valueType == ValueType.Flat)
                    damage.damageMultiplier.AddFlat(buff.amount);
                else //Percentage
                    damage.damageMultiplier.AddPercent(buff.amount);
                break;

            case ItemBuffs.AllProtection:
                if (buff.valueType == ValueType.Flat)
                    protection.allProtection.AddFlat(buff.amount);
                else //Percentage
                    protection.allProtection.AddPercent(buff.amount);
                break;

            case ItemBuffs.ElementalProtection:
                if (buff.valueType == ValueType.Flat)
                    elemental.elementalProtectionMultiplier.AddFlat(buff.amount);
                else //Percentage
                    elemental.elementalProtectionMultiplier.AddPercent(buff.amount);
                break;

            case ItemBuffs.MaxHealth:
                if (buff.valueType == ValueType.Flat)
                    vitals.maxHealth.AddFlat(buff.amount);
                else //Percentage
                    vitals.maxHealth.AddPercent(buff.amount);
                break;

            case ItemBuffs.HealthRegeneration:
                vitals.healthRegeneration += buff.amount;
                break;

            case ItemBuffs.MaxMana:
                if (buff.valueType == ValueType.Flat)
                    vitals.maxMana.AddFlat(buff.amount);
                else //Percentage
                    vitals.maxMana.AddPercent(buff.amount);
                break;

            case ItemBuffs.ManaRegeneration:
                vitals.manaRegeneration += buff.amount;
                break;

            case ItemBuffs.ManaUsage:
                if (buff.valueType == ValueType.Flat)
                    vitals.manaUsageMultiplier.AddFlat(buff.amount);
                else //Percentage
                    vitals.manaUsageMultiplier.AddPercent(buff.amount);
                break;

            case ItemBuffs.Speed:
                movement.speedForce += buff.amount;
                break;
        }
    }

    private void ApplyRuntimeBuff(Buff buff)
    {
        switch (buff.buffType)
        {
            case Buffs.MaxHealth:
                vitals.maxHealth.AddPercent(buff.buffMultiplier);
                break;

            case Buffs.Damage:
                damage.damageMultiplier.AddPercent(buff.buffMultiplier);
                break;

            case Buffs.Protection:
                protection.allProtectionMultiplier.AddPercent(buff.buffMultiplier);
                break;

            case Buffs.MaxMana:
                vitals.maxMana.AddPercent(buff.buffMultiplier);
                break;

            case Buffs.MaxSpeed:
                movement.maxSpeed += buff.buffMultiplier;
                break;
        }
    }
}