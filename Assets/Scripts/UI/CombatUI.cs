using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public List<Button> skillButtons = new();
    public CollisionController _collisionController;

    public PlayerStatsController _playerStats;
    public PlayerStatsController _enemyStats;

    private void Start()
    {
        SetSkillToBTN(0, PlayerController.instance.GetComponent<SkillController>()._skillHolder._skillDatas[0]);
    }

    public void SetSkillToBTN(int buttonIndex, SkillDataParser _skillDataParser)
    {
        skillButtons[buttonIndex].onClick.RemoveAllListeners();
        skillButtons[buttonIndex].onClick.AddListener(() =>
        {
            CombatController _combatController = CombatController.instance;

            _combatController.CastSkill(_skillDataParser, _collisionController);
        });

        //SetSkillBTNData(buttonIndex, _skillDataParser, skillDamage, protection, manaUsage);
    }

    private void SetSkillBTNData(int i, SkillDataParser _skillDataParser, int skillDamage, int protection, int manaUsage)
    {
        skillButtons[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = _skillDataParser.iconSprite;
        skillButtons[i].transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = _skillDataParser._skillData.displayedName;

        //Check if damage or protection
        string info()
        {
            if (skillDamage == 0)
                return protection.ToString();
            else
                return skillDamage.ToString();
        }
        skillButtons[i].transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = $"{info()}";
        //Mana
        skillButtons[i].transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = $"{manaUsage}";
        //Elemental type
        skillButtons[i].transform.GetChild(0).GetChild(4).GetComponent<TMP_Text>().text = $"{_skillDataParser._skillData._skillAttributes[0].elementalTypes}";   
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
