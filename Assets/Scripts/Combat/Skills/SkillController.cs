using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public enum ElementalTypes
{
    NoElement,
    Fire,
    Water,
    Earth,
    Air,
}

public enum AttributeTypes
{
    Buff,

    //Damage
    MeleeDamage,
    RangeDamage,
    MagicDamage,

    //Protection
    AllProtection,
    MeleeProtection,
    RangeProtection,
    MagicProtection,
    
    //Cooldown
    Cooldown,

    //Mana
    ManaUsage,
}

public enum SkillType
{
    Attack,
    Defence,
}

[System.Serializable]
public class Attributes
{
    public AttributeTypes attributeType;
    public ElementalTypes elementalTypes;
    public Buffs buffTypes;
    public int amount;
}

public class SkillController : MonoBehaviour
{
	public SkillHolder _skillHolder;
    public CombatUI _combatUI;
    public Transform skillsParent;
    //public SkillData skillData;

    void Awake()
    {
        for (int i = 0; i < _skillHolder.skillNames.Count; i++)
        {
            _skillHolder._skillDatas[i] = SkillContainer.instance.GetSkillByName(_skillHolder.skillNames[i]);
            
            if (_skillHolder._skillDatas[i] == null)
            {
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no command like: {_skillHolder.skillNames[i]}");
                continue;
            }

            skillsParent.GetChild(i).GetComponent<CollisionController>().Configure(
                TryGetComponent(out PlayerController _) || GetComponent<EntityController>()._entityInfo.entity == Entity.Friendly,
                _skillHolder._skillDatas[i]._skillData
            );
        }

        //Save("");
    }

    public void CastSkill(int index)
    {
        if (_combatUI.skillInfos[index].canBeCasted)
            StartCoroutine(_combatUI.Cast(index, _skillHolder._skillDatas[index]));
    }

    //public virtual void Save(string path)
    //{
    //    //EXAMPLE
    //    //Here save data to file
    //    string jsonData = JsonConvert.SerializeObject(skillData, Formatting.Indented, new JsonSerializerSettings
    //    {
    //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    //        TypeNameHandling = TypeNameHandling.Auto,
    //        PreserveReferencesHandling = PreserveReferencesHandling.None
    //    });

    //    File.WriteAllText(Application.dataPath + "/Game/ala.json", jsonData);
    //}
}
