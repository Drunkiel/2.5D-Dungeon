using UnityEngine;

[System.Serializable]
public class DamageStats
{
    public Stat damageMultiplier = new() { BaseValue = 1f };

    public Stat meleeDamage = new();
    public Stat rangeDamage = new();
    public Stat magicDamage = new();

    public float GetAttributeBonus(AttributeTypes type)
    {
        return type switch
        {
            AttributeTypes.MeleeDamage => meleeDamage.Value / 2f,
            AttributeTypes.RangeDamage => rangeDamage.Value / 2f,
            AttributeTypes.MagicDamage => magicDamage.Value / 2f,
            _ => 0
        };
    }

    public void Reset()
    {
        damageMultiplier.ResetModifiers();
        meleeDamage.ResetModifiers();
        rangeDamage.ResetModifiers();
        magicDamage.ResetModifiers();
    }

    public static int CalculateDamage(
        float baseAmount,
        AttributeTypes attributeType,
        ElementType elementType,
        DamageStats attackerDamage,
        ProtectionStats defenderProtection,
        ElementalStats defenderElemental)
    {
        float damage = baseAmount;

        damage *= attackerDamage.damageMultiplier.Value;

        damage -= defenderProtection.allProtection.Value
                  * (1.5f * defenderProtection.allProtectionMultiplier.Value);

        damage += attackerDamage.GetAttributeBonus(attributeType);

        damage -= defenderProtection.GetAttributeProtection(attributeType)
                  * (1.2f * defenderProtection.allProtectionMultiplier.Value);

        damage -= defenderElemental.GetResistance(elementType)
                  * (2f * defenderElemental.elementalProtectionMultiplier.Value);

        if (damage <= 0)
            damage = 1;

        return Mathf.FloorToInt(damage);
    }
}