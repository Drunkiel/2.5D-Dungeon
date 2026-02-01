using System.Collections.Generic;

public class SkillContainer : SaveLoadSystem
{
    public static SkillContainer instance;

    public List<SkillDataParser> _allSkills = new();

    private void Awake()
    {
        instance = this;
        Load(skillsSavePath);
    }

    public override void Load(string path)
    {
        foreach (SkillDataParser _skillDataParser in _allSkills)
        {
            //Create new skill
            _skillDataParser.sprite = _skillDataParser._skillData.GetSprite(itemsSavePath + "Collectable/" + _skillDataParser._skillData.spritePath, 20f);
            _skillDataParser.iconSprite = _skillDataParser._skillData.GetSprite(itemsSavePath + "Collectable/" + _skillDataParser._skillData.spriteIconPath, 20f);
        }
    }

    public SkillDataParser GetSkillByID(int skillID)
    {
        for (int i = 0; i < _allSkills.Count; i++)
        {
            if (_allSkills[i]._skillData.ID == skillID)
                return _allSkills[i];
        }

        return null;
    }

    public SkillDataParser GetSkillByName(string skillName)
    {
        for (int i = 0; i < _allSkills.Count; i++)
        {
            if (_allSkills[i]._skillData.displayedName == skillName)
                return _allSkills[i];
        }

        return null;
    }
}
