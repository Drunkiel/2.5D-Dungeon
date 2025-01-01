using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestController : MonoBehaviour
{
    public static QuestController instance;
    public QuestUI _questUI;

    public List<Quest> _allQuests;
    public List<int> _currentQuestsIndex;

    public short idToCheck;
    public UnityEvent killEvent;
    public UnityEvent collectEvent;
    public UnityEvent talkEvent;

    private void Awake()
    {
        instance = this;
        GetQuest(0);
        GetQuest(1);
        GetQuest(2);
    }

    public void GetQuest(int questIndex)
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
                    killEvent.AddListener(() =>
                    {
                        int a = i - 1;
                        _allQuests[questIndex]._requirements[a].UpdateStatus(idToCheck);
                        _questUI.UpdateQuestUI(questIndex, a, _allQuests[questIndex]);
                    });
                    break;

                case RequirementType.Collect:
                    collectEvent.AddListener(() =>
                    {
                        int a = i - 1;
                        _allQuests[questIndex]._requirements[a].UpdateStatus(idToCheck);
                        _questUI.UpdateQuestUI(questIndex, a, _allQuests[questIndex]);
                    });
                    break;

                case RequirementType.Talk:
                    talkEvent.AddListener(() =>
                    {
                        int a = i - 1;
                        _allQuests[questIndex]._requirements[a].UpdateStatus(idToCheck);
                        _questUI.UpdateQuestUI(questIndex, a, _allQuests[questIndex]);
                    });
                    break;
            }
        }
    }

    public void InvokeKillEvent(short id)
    {
        idToCheck = id;
        killEvent.Invoke();
    }

    public void InvokeCollectEvent(short id)
    {
        idToCheck = id;
        collectEvent.Invoke();
    }

    public void InvokeTalkEvent(short id)
    {
        idToCheck = id;
        talkEvent.Invoke();
    }
}
