using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SkillContainer : SaveLoadSystem
{
    public static SkillContainer instance;

    public List<SkillDataParser> _allSkills = new();
    public SkillData skillData;

    private void Awake()
    {
        instance = this;
        Load(skillsSavePath);
    }

    public override void Load(string path)
    {
        List<string> skillsLocation = GetGameFiles(path);

        foreach (var skillLocation in skillsLocation)
        {
            string saveFile = ReadFromFile($"{path}/{skillLocation}");

            //Create new skill
            SkillData _skillData = ScriptableObject.CreateInstance<SkillData>();
            JsonConvert.PopulateObject(saveFile, _skillData);
            SkillDataParser _skillDataParser = ScriptableObject.CreateInstance<SkillDataParser>();
            _skillDataParser._skillData = _skillData;
            _skillDataParser.sprite = _skillData.GetSprite(itemsSavePath + "Collectable/" + _skillData.spritePath, 20f);
            _skillDataParser.iconSprite = _skillData.GetSprite(itemsSavePath + "Collectable/" + _skillData.spriteIconPath, 20f);
            _allSkills.Add(_skillDataParser);
        }
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
