using System;
using UnityEngine;

[Serializable]
public class Stat
{
    public float BaseValue;

    [SerializeField] private float flatBonus;
    [SerializeField] private float percentBonus;

    public float FlatBonus => flatBonus;
    public float PercentBonus => percentBonus;

    public float Value => (BaseValue + flatBonus) * (1f + percentBonus);

    public void AddFlat(float amount)
    {
        flatBonus += amount;
    }

    public void AddPercent(float percent)
    {
        percentBonus += percent;
    }

    public void ResetModifiers()
    {
        flatBonus = 0;
        percentBonus = 0;
    }
}