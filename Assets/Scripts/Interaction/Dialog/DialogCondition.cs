using UnityEngine;

public abstract class DialogCondition : ScriptableObject
{
    public abstract bool CheckIfTrue();
}