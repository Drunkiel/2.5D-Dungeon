using System;
using System.Collections;
using System.Collections.Generic;
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
    public List<SkillInfo> _skillInfos = new();

    private void Start()
    {
        for (int i = 0; i < _skillInfos.Count; i++)
            if (_skillInfos[i].skillButton != null)
            {
                SetSkillToBTN(i, PlayerController.instance.GetComponent<SkillController>()._skillHolder._skillDatas[i]);
                _skillInfos[i]._collisionController = PlayerController.instance.GetComponent<SkillController>().skillsParent.GetChild(i).GetComponent<CollisionController>();
            }
    }

    public void SetSkillToBTN(int buttonIndex, SkillDataParser _skillDataParser)
    {
        _skillInfos[buttonIndex].skillButton.onClick.RemoveAllListeners();
        _skillInfos[buttonIndex].skillButton.onClick.AddListener(() =>
        {
            if (_skillInfos[buttonIndex].canBeCasted)
                StartCoroutine(Cast(buttonIndex, _skillDataParser));
        });
    }

    public IEnumerator Cast(int buttonIndex, SkillDataParser _skillDataParser)
    {
        CombatController _combatController = CombatController.instance;
        StartCoroutine(_combatController.CastSkill(_skillDataParser, _skillInfos[buttonIndex]._collisionController));
        _skillInfos[buttonIndex].canBeCasted = false;
        if (_skillInfos[buttonIndex].skillButton != null)
            _skillInfos[buttonIndex].skillButton.interactable = false;

        // Wait until the animation is done
        yield return new WaitForSeconds(GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }));

        _skillInfos[buttonIndex].canBeCasted = true;
        if (_skillInfos[buttonIndex].skillButton != null)
            _skillInfos[buttonIndex].skillButton.interactable = true;
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
