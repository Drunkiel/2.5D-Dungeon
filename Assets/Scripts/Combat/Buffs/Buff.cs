using UnityEngine;

public enum Buffs
{
    MaxHealth,
    MaxMana,
    Damage,
    Protection,
    MaxSpeed,
}

[System.Serializable]
public class Buff
{
    public string name; 
    public float duration; 
    public float timer;
    public Buffs buffType;
    public Sprite sprite;
    public int buffMultiplier;
    public bool isPermanent = false;

    public Buff(string name, float duration, Buffs buffType, int buffMultiplier, bool isPermanent = false)
    {
        this.name = name;
        this.duration = duration;
        this.buffType = buffType;
        this.sprite = null;
        this.buffMultiplier = buffMultiplier;
        this.isPermanent = isPermanent;
        this.timer = 0f;
    }

    //Checks if buff is still active
    public bool UpdateBuff()
    {
        if (!isPermanent)
        {
            timer += Time.deltaTime;
            return timer < duration;
        }
        return true;
    }
}
