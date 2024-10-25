using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkillInfo
{
    public Button skillButton;
    public CollisionController _collisionController;
    public bool canBeCasted = true;
}

public class CombatUI : MonoBehaviour
{
    public List<SkillInfo> skillInfos = new();

    private void Start()
    {
        SetSkillToBTN(0, PlayerController.instance.GetComponent<SkillController>()._skillHolder._skillDatas[0]);
    }

    public void SetSkillToBTN(int buttonIndex, SkillDataParser _skillDataParser)
    {
        skillInfos[buttonIndex].skillButton.onClick.RemoveAllListeners();
        skillInfos[buttonIndex].skillButton.onClick.AddListener(() =>
        {
            if (skillInfos[buttonIndex].canBeCasted)
                StartCoroutine(Cast(buttonIndex, _skillDataParser));
        });

        //SetSkillBTNData(buttonIndex, _skillDataParser, skillDamage, protection, manaUsage);
    }

    private void SetSkillBTNData(int i, SkillDataParser _skillDataParser, int skillDamage, int protection, int manaUsage)
    {
        skillInfos[i].skillButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _skillDataParser.iconSprite;
        skillInfos[i].skillButton.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = _skillDataParser._skillData.displayedName;

        //Check if damage or protection
        string info()
        {
            if (skillDamage == 0)
                return protection.ToString();
            else
                return skillDamage.ToString();
        }
        skillInfos[i].skillButton.transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = $"{info()}";
        //Mana
        skillInfos[i].skillButton.transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = $"{manaUsage}";
        //Elemental type
        skillInfos[i].skillButton.transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = $"{_skillDataParser._skillData._skillAttributes[0].elementalTypes}";   
    }

    private IEnumerator Cast(int buttonIndex, SkillDataParser _skillDataParser)
    {
        CombatController _combatController = CombatController.instance;
        _combatController.CastSkill(_skillDataParser, skillInfos[buttonIndex]._collisionController);
        skillInfos[buttonIndex].canBeCasted = false;
        skillInfos[buttonIndex].skillButton.interactable = false;

        // Wait until the animation is done
        yield return new WaitForSeconds(GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }));

        skillInfos[buttonIndex].canBeCasted = true;
        skillInfos[buttonIndex].skillButton.interactable = true;
    }

    public int GetSkillModifier(SkillData _skillData, List<AttributeTypes> attributeTypes)
    {
        for (int i = 0; i < _skillData._skillAttributes.Count; i++)
        {
            for (int j = 0; j < attributeTypes.Count; j++)
            {
                if (_skillData._skillAttributes[i].attributeType == attributeTypes[j])
                    return _skillData._skillAttributes[i].amount;
            }
        }

        return 0;
    }
}
