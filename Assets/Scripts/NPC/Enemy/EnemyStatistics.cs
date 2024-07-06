using UnityEngine;

[System.Serializable]
public class EnemyStatistics
{
    public int health;
    public int maxHealth;
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
