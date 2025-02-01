using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PopUpInfo
{
    QuestAccepted,
    QuestCompleted,
    VisitPlace,
}

[System.Serializable]
public class PopUp
{
    public PopUpInfo info;
    public string text;

    public PopUp(PopUpInfo info, string text)
    {
        this.info = info;
        this.text = text;
    }
}

public class PopUpController : MonoBehaviour
{
    public static PopUpController instance;

    private Queue<PopUp> _popUpQueue = new();
    public bool isPopUpRunning;

    [SerializeField] private TMP_Text text;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        instance = this;
    }

    public void CreatePopUp(PopUpInfo info, string text)
    {
        _popUpQueue.Enqueue(new PopUp(info, text));
        if (!isPopUpRunning)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        while (_popUpQueue.Count > 0)
            yield return StartCoroutine(RunPopUp(_popUpQueue.Dequeue()));
    }

    private IEnumerator RunPopUp(PopUp _popUp)
    {
        isPopUpRunning = true;

        string newText = _popUp.info switch
        {
            PopUpInfo.QuestAccepted => $"Quest accepted\n<color=yellow>{_popUp.text}</color>",
            PopUpInfo.QuestCompleted => $"Quest completed\n<color=yellow>{_popUp.text}</color>",
            PopUpInfo.VisitPlace => $"<color=yellow>{_popUp.text}</color>",
            _ => ""
        };

        text.text = newText;
        animator.Play("Display");
        yield return new WaitForSeconds(3f);
        isPopUpRunning = false;
    }
}
