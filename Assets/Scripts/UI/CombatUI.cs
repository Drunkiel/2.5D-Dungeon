using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public List<Button> skillButtons = new();
    public List<Button> optionsButtons = new();

    public void SetSkillToBTN(int buttonIndex, SkillData _skillData)
    {
        if (_skillData == null)
        {
            skillButtons[buttonIndex].interactable = false;
            return;
        }
        else
            skillButtons[buttonIndex].interactable = true;

        skillButtons[buttonIndex].onClick.RemoveAllListeners();
        skillButtons[buttonIndex].onClick.AddListener(() =>
        {
            CombatController _combatController = CombatController.instance;
            if (!_combatController.IsPlayerTurn())
                return;

            int skillDamage = GetSkillModifier(_skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
            int protection = GetSkillModifier(_skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
            int manaUsage = GetSkillModifier(_skillData, new() { AttributeTypes.ManaUsage });

            //Taking turn
            _combatController.TakeTurn(() =>
            {
                EntityStatistics _enemyStatistics = CombatEntities.instance.enemy.GetComponent<EnemyController>()._statistics;
                Attributes _attributes = _skillData._skillAttributes[0];
                _enemyStatistics.TakeDamage(skillDamage, _attributes.elementalTypes);
                _enemyStatistics.TakeMana(manaUsage);
            });
        });

        SetSkillBTNData(_skillData);
    }

    private void SetSkillBTNData(SkillData _skillData)
    {
        int skillDamage = GetSkillModifier(_skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
        int protection = GetSkillModifier(_skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
        int manaUsage = GetSkillModifier(_skillData, new() { AttributeTypes.ManaUsage });

        for (int i = 0; i < skillButtons.Count; i++)
        {
            skillButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = _skillData.skillIconSprite;
            skillButtons[i].transform.GetChild(1).GetComponent<TMP_Text>().text = _skillData.skillName;
            //Mana
            skillButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = $"{manaUsage}";
            //Elemental type
            skillButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = $"{_skillData._skillAttributes[0].elementalTypes}";   
        }
    }

    private int GetSkillModifier(SkillData _skillData, List<AttributeTypes> attributeTypes)
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
