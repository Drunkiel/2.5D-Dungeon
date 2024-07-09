using UnityEngine;

[System.Serializable]
public class EnemyStatistics
{
    public int health;
    public int maxHealth;

    public int allProtection;
    public int meleeProtection;
    public int rangeProtection;
    public int magicProtection;

    public int mana;
    public int maxMana;

    public float speedForce;
    public readonly float maxSpeed = 1.2f;

    public float jumpForce;

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
            Debug.Log("Enemy died ;<");
        }
    }

    public int CalculateDamage(int amount, ElementalTypes elementalTypes)
    {
        float damageOutput = amount;

        damageOutput -= allProtection / 2;
        damageOutput -= meleeProtection / 1.5f;
        damageOutput -= rangeProtection / 1.5f;
        damageOutput -= magicProtection / 1.5f;

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
            Debug.Log("Enemy has no more mana ;<");
        }
    }
}
