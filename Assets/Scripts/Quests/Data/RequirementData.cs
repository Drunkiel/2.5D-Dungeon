using System;

[Serializable]
public class RequirementData
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
    }
}