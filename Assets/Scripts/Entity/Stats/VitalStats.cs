using UnityEngine;

[System.Serializable]
public class VitalStats
{
    public Stat maxHealth = new();
    public Stat maxMana = new();

    public int health;
    public int mana;

    public float healthRegeneration;
    public float manaRegeneration;

    public Stat manaUsageMultiplier = new() { BaseValue = 1f };

    public void Initialize()
    {
        health = Mathf.FloorToInt(maxHealth.Value);
        mana = Mathf.FloorToInt(maxMana.Value);
    }
}