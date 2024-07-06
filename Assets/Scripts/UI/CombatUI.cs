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

            //Taking turn
            _combatController.TakeTurn(() =>
            {
                CombatEntities.instance.enemy.GetComponent<EnemyController>()._statistics.TakeDamage(1);
                print($"Used skill on index: {buttonIndex}");
            });
        });
        skillButtons[buttonIndex].transform.GetChild(0).GetComponent<Image>().sprite = _skillData.skillIconSprite;
        skillButtons[buttonIndex].transform.GetChild(1).GetComponent<TMP_Text>().text = _skillData.skillName;
    }
}
