using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControlRoom : MonoBehaviour
{
    public List<bool> requirementsList = new();
    public List<GameObject> progressIndicators = new();

    public UnityEvent onFinishEvent;

    public void ManageProgress(int index)
    {
        requirementsList[index] = !requirementsList[index];
        progressIndicators[index].SetActive(requirementsList[index]);

        if (CheckIfFinished())
            onFinishEvent.Invoke();
    }

    private bool CheckIfFinished()
    {
        for (int i = 0; i < requirementsList.Count; i++)
            if (!requirementsList[i])
                return false;

        return true;
    }
}
