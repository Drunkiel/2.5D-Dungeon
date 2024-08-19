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

        int skillDamage = GetSkillModifier(_skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
        int protection = GetSkillModifier(_skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
        int manaUsage = GetSkillModifier(_skillData, new() { AttributeTypes.ManaUsage });

        skillButtons[buttonIndex].onClick.RemoveAllListeners();
        skillButtons[buttonIndex].onClick.AddListener(() =>
        {
            CombatController _combatController = CombatController.instance;
            if (!_combatController.IsPlayerTurn())
                return;

            //Taking turn
            _combatController.TakeTurn(() =>
            {
                EntityStatistics _enemyStatistics = CombatEntities.instance.enemy.GetComponent<EnemyController>()._statistics;
                EntityStatistics _playerStatistics = PlayerController.instance._statistics;

                //Checks if player has enough mana to cast skill
                if (_playerStatistics.mana * _playerStatistics.manaUsageMultiplier < manaUsage)
                {
                    print($"Not enough mana: {Mathf.Abs(_playerStatistics.mana - manaUsage)}");
                    return;
                }

                //Do stuff
                Attributes _attributes = _skillData._skillAttributes[0];
                int playerDamage = 0;
                switch (_attributes.attributeType)
                {
                    case AttributeTypes.MeleeDamage:
                        playerDamage = _playerStatistics.meleeDamage;
                        break;
                    
                    case AttributeTypes.RangeDamage:
                        playerDamage = _playerStatistics.rangeDamage;
                        break;

                    case AttributeTypes.MagicDamage:
                        playerDamage = _playerStatistics.magicDamage;
                        break;
                }

                _enemyStatistics.TakeDamage((skillDamage + playerDamage) * _playerStatistics.damageMultiplier, _attributes.attributeType, _attributes.elementalTypes);
                _playerStatistics.TakeMana(manaUsage);
            });
        });

        SetSkillBTNData(_skillData, skillDamage, protection, manaUsage);
    }

    private void SetSkillBTNData(SkillData _skillData, int skillDamage, int protection, int manaUsage)
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            skillButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            skillButtons[i].transform.GetChild(1).GetComponent<TMP_Text>().text = _skillData.displayedName;
            //Damage
            skillButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = $"{skillDamage}";
            //Mana
            skillButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = $"{manaUsage}";
            //Elemental type
            skillButtons[i].transform.GetChild(4).GetComponent<TMP_Text>().text = $"{_skillData._skillAttributes[0].elementalTypes}";   
        }
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
