[System.Serializable]
public class ElementalStats
{
    public Stat elementalProtectionMultiplier = new() { BaseValue = 1f };

    public Stat fireResistance = new();
    public Stat waterResistance = new();
    public Stat earthResistance = new();
    public Stat airResistance = new();

    public float GetResistance(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => fireResistance.Value,
            ElementType.Water => waterResistance.Value,
            ElementType.Earth => earthResistance.Value,
            ElementType.Air => airResistance.Value,
            _ => 0
        };
    }

    public void Reset()
    {
        elementalProtectionMultiplier.ResetModifiers();
        fireResistance.ResetModifiers();
        waterResistance.ResetModifiers();
        earthResistance.ResetModifiers();
        airResistance.ResetModifiers();
    }
}