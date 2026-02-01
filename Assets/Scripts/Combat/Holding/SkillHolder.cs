using System.Collections.Generic;

[System.Serializable]
public class SkillHolder
{
    public List<int> skillsID = new();
    public List<SkillDataParser> _skillDatas = new();
}
