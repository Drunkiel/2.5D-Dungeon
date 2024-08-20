using System.Collections.Generic;

[System.Serializable]
public class SkillHolder
{
    public List<string> skillNames = new();
    public List<SkillDataParser> _skillDatas = new(6);
}
