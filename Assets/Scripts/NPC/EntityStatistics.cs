using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStatistics
{
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;

    public float damageMultiplier = 1;
    public float protectionMultiplier = 1;
    public float manaUsageMultiplier = 1;

    public int meleeDamage;
    public int rangeDamage;
    public int magicDamage;

    public int allProtection;
    public int meleeProtection;
    public int rangeProtection;
    public int magicProtection;

    public int fireResistance;
    public int waterResistance;
    public int earthResistance;
    public int airResistance;

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

        damageOutput -= allProtection * (1.5f * protectionMultiplier);

        switch(attributeTypes)
        {
            case AttributeTypes.MeleeDamage:
                damageOutput -= meleeProtection * (1.2f * protectionMultiplier);
                break;

            case AttributeTypes.RangeDamage:
                damageOutput -= rangeProtection * (1.2f * protectionMultiplier);
                break;

            case AttributeTypes.MagicDamage:
                damageOutput -= magicProtection * (1.2f * protectionMultiplier);
                break;
        }

        switch (elementalTypes)
        {
            case ElementalTypes.NoElement:
                damageOutput -= 0;
                break;

            case ElementalTypes.Fire:
                damageOutput -= fireResistance * (2 * protectionMultiplier);
                break;

            case ElementalTypes.Water:
                damageOutput -= waterResistance * (2 * protectionMultiplier);
                break;

            case ElementalTypes.Earth:
                damageOutput -= earthResistance * (2 * protectionMultiplier);
                break;

            case ElementalTypes.Air:
                damageOutput -= airResistance * (1.25f * protectionMultiplier);
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
        meleeDamage = 0;
        rangeDamage = 0;
        magicDamage = 0;
        allProtection = 0;
        meleeProtection = 0;
        rangeProtection = 0;
        magicProtection = 0;
    }
}
