using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStatistics
{
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;

    public float damageMultiplier;
    public float protectionMultiplier;
    public float manaUsageMultiplier;

    public int allProtection;
    public int meleeProtection;
    public int rangeProtection;
    public int magicProtection;

    public int fireResistance;
    public int waterResistance;
    public int earthResistance;
    public int airResistance;

    public float speedForce;
    public readonly float maxSpeed = 1.2f;

    public float jumpForce;
    public List<bool> additionalJumps = new();

    public void TakeDamage(int amount, ElementalTypes elementalTypes)
    {
        int damageToDeal = CalculateDamage(amount * damageMultiplier, elementalTypes);

        health -= damageToDeal;
        if (health < 0)
        {
            health = 0;
            Debug.Log("Enemy died ;<");
        }
    }

    public int CalculateDamage(float amount, ElementalTypes elementalTypes)
    {
        float damageOutput = amount;

        damageOutput -= allProtection * (1.5f * protectionMultiplier);
        damageOutput -= meleeProtection * (1.2f * protectionMultiplier);
        damageOutput -= rangeProtection * (1.2f * protectionMultiplier);
        damageOutput -= magicProtection * (1.2f * protectionMultiplier);

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

        Debug.Log(damageOutput);

        return Mathf.FloorToInt(damageOutput);
    }

    public void TakeMana(int amount)
    {
        mana -= amount;
        if (mana < 0)
        {
            mana = 0;
            Debug.Log("Player has no more mana ;<");
        }
    }
}
