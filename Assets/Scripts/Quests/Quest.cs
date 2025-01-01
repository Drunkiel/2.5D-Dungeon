using System;
using System.Collections.Generic;

[Serializable]
public class Quest
{
    public float id;
    public string title;
    public string description;
    public List<Requirement> _requirements;
}

[Serializable]
public class Requirement
{
    public RequirementType type;
    public string description;
    public short targetID;
    public int progressCurrent;
    public int progressNeeded;

    public void UpdateStatus(short id)
    {
        if (id != targetID)
            return;

        progressCurrent += 1;
        if (progressCurrent >= progressNeeded)
            ConsoleController.instance.ChatMessage(SenderType.Hidden, "Passed");
    }
}

public enum RequirementType
{
    Kill,
    Collect,
    Talk,
}