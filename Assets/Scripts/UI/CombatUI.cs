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
        for (int i = 0; i < skillInfos.Count; i++)
            if (skillInfos[i].skillButton != null)
                SetSkillToBTN(i, PlayerController.instance.GetComponent<SkillController>()._skillHolder._skillDatas[i]);
    }

    public void SetSkillToBTN(int buttonIndex, SkillDataParser _skillDataParser)
    {
        skillInfos[buttonIndex].skillButton.onClick.RemoveAllListeners();
        skillInfos[buttonIndex].skillButton.onClick.AddListener(() =>
        {
            if (skillInfos[buttonIndex].canBeCasted)
                StartCoroutine(Cast(buttonIndex, _skillDataParser));
        });
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

    public IEnumerator Cast(int buttonIndex, SkillDataParser _skillDataParser)
    {
        CombatController _combatController = CombatController.instance;
        _combatController.CastSkill(_skillDataParser, skillInfos[buttonIndex]._collisionController);
        skillInfos[buttonIndex].canBeCasted = false;
        if (skillInfos[buttonIndex].skillButton != null)
            skillInfos[buttonIndex].skillButton.interactable = false;

        // Wait until the animation is done
        yield return new WaitForSeconds(GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }));

        skillInfos[buttonIndex].canBeCasted = true;
        if (skillInfos[buttonIndex].skillButton != null)
            skillInfos[buttonIndex].skillButton.interactable = true;
    }

    public float GetSkillModifier(SkillData _skillData, List<AttributeTypes> attributeTypes)
    {
        for (int i = 0; i < _skillData._skillAttributes.Count; i++)
        {
            for (int j = 0; j < attributeTypes.Count; j++)
            {
                if (_skillData._skillAttributes[i].attributeType == attributeTypes[j])
                {
                    //Check if cooldown
                    if (_skillData._skillAttributes[i].attributeType != AttributeTypes.Cooldown)
                        return _skillData._skillAttributes[i].amount;
                    else
                        return _skillData._skillAttributes[i].amount / 100f;
                }
            }
        }

        return 0;
    }

    public Buffs GetBuff(SkillDataParser _skillDataParser)
    {
        for (int i = 0; i < _skillDataParser._skillData._skillAttributes.Count; i++)
        {
            if (_skillDataParser._skillData._skillAttributes[i].buffTypes != Buffs.None)
                return _skillDataParser._skillData._skillAttributes[i].buffTypes;
        }

        return Buffs.None;  
    }
}
