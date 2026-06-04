using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Conditions/Test")]
public class TestCondition : DialogCondition
{
    public int a;

    public override bool CheckIfTrue()
    {
        if (a > 0)
            return true;
        else
            return false;
    }
}
