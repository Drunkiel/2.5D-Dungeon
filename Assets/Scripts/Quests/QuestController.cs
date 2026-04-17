using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController instance;
    public QuestUI _questUI;

    public List<Quest> _allQuests;
    public List<int> _currentQuestsIndex;

    public short idToCheck;
    public List<short> killQuestIndexes;
    public List<short> collectQuestIndexes;
    public List<short> talkQuestIndexes;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GiveQuest(0, false);
    }

    public void GiveQuest(int questIndex)
    {
        GiveQuest(questIndex, true);
    }

    public void GiveQuest(int questID, bool showPopUp)
    {
        //Check if index is bigger than are quests
        if (questID >= _allQuests.Count)
            return;

        //Check if quest is activated
        if (_currentQuestsIndex.Contains(questID) || _allQuests[questID].CheckIfFinished())
            return;

        //Add quest
        _currentQuestsIndex.Add(questID);
        _questUI.AddQuestToUI(_allQuests[questID]);

        if (PopUpController.instance != null && showPopUp)
            PopUpController.instance.CreatePopUp(PopUpInfo.QuestAccepted, _allQuests[questID].title);

        //Set listeners
        for (int i = 0; i < _allQuests[questID]._requirements.Count; i++)
        {
            switch (_allQuests[questID]._requirements[i].type)
            {
                case RequirementType.Kill:
                    killQuestIndexes.Add((short)questID);
                    break;

                case RequirementType.Collect:
                    collectQuestIndexes.Add((short)questID);
                    break;

                case RequirementType.Talk:
                    talkQuestIndexes.Add((short)questID);
                    break;
            }
        }
    }

    private void FinishQuest(int questIndex)
    {
        //Removing quest
        _questUI.RemoveQuestUI(questIndex);
        _currentQuestsIndex.Remove(questIndex);
        PopUpController.instance.CreatePopUp(PopUpInfo.QuestCompleted, _allQuests[questIndex].title);
        _allQuests[questIndex].onFinishEvent.Invoke();

        for (int i = 0; i < _allQuests[questIndex]._requirements.Count; i++)
        {
            //Reset progress
            _allQuests[questIndex]._requirements[i].progressCurrent = 0;

            //Remove from listeners
            switch (_allQuests[questIndex]._requirements[i].type)
            {
                case RequirementType.Kill:
                    killQuestIndexes.Remove((short)questIndex);
                    break;

                case RequirementType.Collect:
                    collectQuestIndexes.Remove((short)questIndex);
                    break;

                case RequirementType.Talk:
                    talkQuestIndexes.Remove((short)questIndex);
                    break;
            }
        }
    }

    public void InvokeKillEvent(short id)
    {
        idToCheck = id;
        for (int i = 0; i < killQuestIndexes.Count; i++)
        {
            //Simple check
            if (killQuestIndexes[i] < 0 || killQuestIndexes[i] >= _allQuests.Count)
                continue;

            //Get correct RequirementType
            var requirements = _allQuests[killQuestIndexes[i]]._requirements;
            for (int j = 0; j < requirements.Count; j++)
            {
                if (requirements[j].type != RequirementType.Kill)
                    continue;

                EventListener(killQuestIndexes[i], j);
                break;
            }
        }
    }

    public void InvokeCollectEvent(short id)
    {
        idToCheck = id;
        for (int i = 0; i < collectQuestIndexes.Count; i++)
        {
            //Simple check
            if (collectQuestIndexes[i] < 0 || collectQuestIndexes[i] >= _allQuests.Count)
                continue;

            //Get correct RequirementType
            var requirements = _allQuests[collectQuestIndexes[i]]._requirements;
            for (int j = 0; j < requirements.Count; j++)
            {
                if (requirements[j].type != RequirementType.Collect)
                    continue;

                EventListener(collectQuestIndexes[i], j);
                break;
            }
        }
    }

    public void InvokeTalkEvent(short id)
    {
        idToCheck = id;
        for (int i = 0; i < talkQuestIndexes.Count; i++)
        {
            //Simple check
            if (talkQuestIndexes[i] < 0 || talkQuestIndexes[i] >= _allQuests.Count)
                continue;

            //Get correct RequirementType
            var requirements = _allQuests[talkQuestIndexes[i]]._requirements;
            for (int j = 0; j < requirements.Count; j++)
            {
                if (requirements[j].type != RequirementType.Talk)
                    continue;

                EventListener(talkQuestIndexes[i], j);
                break;
            }
        }
    }

    private void EventListener(int questIndex, int i)
    {
        //Update ui
        _allQuests[questIndex]._requirements[i].UpdateStatus(idToCheck);
        _questUI.UpdateQuestUI(questIndex, i, _allQuests[questIndex]);

        //Check if quest is finished
        if (_allQuests[questIndex].CheckIfFinished())
            FinishQuest(questIndex);
    }
}
