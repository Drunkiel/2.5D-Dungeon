using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestController : MonoBehaviour
{
    public static QuestController instance;
    public QuestUI _questUI;

    public List<Quest> _allQuests;

    public UnityEvent killEvent;
    public UnityEvent collectEvent;
    public UnityEvent talkEvent;

    private void Awake()
    {
        instance = this;
        _questUI.AddQuestToUI(_allQuests[0]);
    }

    public void InvokeKillEvent()
    {
        killEvent.Invoke();
    }

    public void InvokeCollectEvent()
    {
        killEvent.Invoke();
    }

    public void InvokeTalkEvent()
    {
        killEvent.Invoke();
    }
}
