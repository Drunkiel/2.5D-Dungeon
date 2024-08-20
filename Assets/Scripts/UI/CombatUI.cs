using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public List<Button> skillButtons = new();
    public List<Button> optionsButtons = new();

    public void SetSkillToBTN(int buttonIndex, SkillDataParser _skillDataParser)
    {
        int skillDamage = GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
        int protection = GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
        int manaUsage = GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.ManaUsage });

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

                //Checks what type of damage to deal
                Attributes _attributes = _skillDataParser._skillData._skillAttributes[0];
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

        SetSkillBTNData(buttonIndex, _skillDataParser, skillDamage, protection, manaUsage);
    }

    private void SetSkillBTNData(int i, SkillDataParser _skillDataParser, int skillDamage, int protection, int manaUsage)
    {
        skillButtons[i].transform.GetChild(0).GetComponent<Image>().sprite = _skillDataParser.iconSprite;
        skillButtons[i].transform.GetChild(1).GetComponent<TMP_Text>().text = _skillDataParser._skillData.displayedName;
        //Check if damage or protection
        string info()
        {
            if (skillDamage == 0)
                return protection.ToString();
            else
                return skillDamage.ToString();
        }
        skillButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = $"{info()}";
        //Mana
        skillButtons[i].transform.GetChild(3).GetComponent<TMP_Text>().text = $"{manaUsage}";
        //Elemental type
        skillButtons[i].transform.GetChild(4).GetComponent<TMP_Text>().text = $"{_skillDataParser._skillData._skillAttributes[0].elementalTypes}";   
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
