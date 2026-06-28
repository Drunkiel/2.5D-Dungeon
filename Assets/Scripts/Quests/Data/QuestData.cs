using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RequirementType
{
    Kill,
    Collect,
    Talk,
}

[Serializable]
[CreateAssetMenu(menuName = "Custom/Quest/New Quest")]
public class QuestData : ScriptableObject
{
    public short id;
    public string title;
    public string description;
    public List<RequirementData> _requirements;
    public UnityEvent onFinishEvent;

    public bool CheckIfFinished()
    {
        for (int i = 0; i < _requirements.Count; i++)
        {
            if (_requirements[i].progressCurrent < _requirements[i].progressNeeded)
                return false;
        }

        return true;
    }
}