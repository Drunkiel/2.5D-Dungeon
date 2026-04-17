using TMPro;
using UnityEngine;

public class QuestExpander : MonoBehaviour
{
    [SerializeField] private TMP_Text questTitleText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private OpenCloseUI _openCloseUI;

    public void SetExpander(Quest quest)
    {
        _openCloseUI.Open();

        questTitleText.text = quest.title;
        questDescriptionText.text = quest.description;
    }
}
