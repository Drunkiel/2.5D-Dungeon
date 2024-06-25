using System.Collections.Generic;

[System.Serializable]
public class PlayerStatistics
{
    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;

    public float speedForce;
    public readonly float maxSpeed = 1.2f;

    public float jumpForce;
    public List<bool> additionalJumps = new();
}
