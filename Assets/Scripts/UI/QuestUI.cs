using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public GameObject questPrefab;
    public GameObject requirementPrefab;
    public Transform parent;

    public void AddQuestToUI(Quest _quest)
    {
        GameObject newQuest = Instantiate(questPrefab, parent);
        for (int i = 0; i < _quest._requirements.Count; i++)
        {
            GameObject newRequirement = Instantiate(requirementPrefab, newQuest.transform.GetChild(1));
            TMP_Text descriptionText = newRequirement.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text progressText = newRequirement.transform.GetChild(2).GetComponent<TMP_Text>();

            descriptionText.text = _quest._requirements[i].description;
            progressText.text = $"{_quest._requirements[i].progressCurrent} / {_quest._requirements[i].progressNeeded}";
        }

        //Set title
        TMP_Text titleText = newQuest.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        titleText.text = _quest.title;
    }
}
