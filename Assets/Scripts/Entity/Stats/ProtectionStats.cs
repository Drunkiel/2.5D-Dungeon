[System.Serializable]
public class ProtectionStats
{
    public Stat allProtectionMultiplier = new() { BaseValue = 1f };

    public Stat allProtection = new();
    public Stat meleeProtection = new();
    public Stat rangeProtection = new();
    public Stat magicProtection = new();

    public float GetAttributeProtection(AttributeTypes type)
    {
        return type switch
        {
            AttributeTypes.MeleeDamage => meleeProtection.Value,
            AttributeTypes.RangeDamage => rangeProtection.Value,
            AttributeTypes.MagicDamage => magicProtection.Value,
            _ => 0
        };
    }

    public void Reset()
    {
        allProtectionMultiplier.ResetModifiers();
        allProtection.ResetModifiers();
        meleeProtection.ResetModifiers();
        rangeProtection.ResetModifiers();
        magicProtection.ResetModifiers();
    }
}