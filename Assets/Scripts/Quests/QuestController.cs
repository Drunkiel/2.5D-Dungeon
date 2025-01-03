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

    public void GiveQuest(int questIndex)
    {
        //Check if index is bigger than are quests
        if (questIndex >= _allQuests.Count)
            return;

        //Check if quest is activated
        if (_currentQuestsIndex.Contains(questIndex))
            return;

        //Add quest
        _currentQuestsIndex.Add(questIndex);
        _questUI.AddQuestToUI(_allQuests[questIndex]);

        for (int i = 0; i < _allQuests[questIndex]._requirements.Count; i++)
        {
            switch (_allQuests[questIndex]._requirements[i].type)
            {
                case RequirementType.Kill:
                    killQuestIndexes.Add((short)questIndex);
                    break;

                case RequirementType.Collect:
                    collectQuestIndexes.Add((short)questIndex);
                    break;

                case RequirementType.Talk:
                    talkQuestIndexes.Add((short)questIndex);
                    break;
            }
        }
    }

    private void FinishQuest(int questIndex)
    {
        _questUI.RemoveQuestUI(questIndex);
        _currentQuestsIndex.Remove(questIndex);
        _allQuests[questIndex].onFinishEvent.Invoke();

        for (int i = 0; i < _allQuests[questIndex]._requirements.Count; i++)
        {
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

    private void EventListener(int questIndex, int i)
    {
        _allQuests[questIndex]._requirements[i].UpdateStatus(idToCheck);
        _questUI.UpdateQuestUI(questIndex, i, _allQuests[questIndex]);

        if (_allQuests[questIndex].CheckIfFinished())
            FinishQuest(questIndex);
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
                if (requirements[j].type != RequirementType.Talk)
                    continue;

                EventListener(killQuestIndexes[i], j);
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
                if (requirements[j].type != RequirementType.Talk)
                    continue;

                EventListener(collectQuestIndexes[i], j);
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
            }
        }
    }
}
