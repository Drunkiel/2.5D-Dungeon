using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControlRoom : MonoBehaviour
{
    public List<bool> requirementsList = new();
    public List<GameObject> progressIndicators = new();
    public List<Material> materials = new();

    public UnityEvent onFinishEvent;

    void Start()
    {
        //Activate lights that gonna be displayed (defaultly they are not visible)
        for (int i = 0; i < progressIndicators.Count; i++)
            progressIndicators[i].SetActive(true);
    }

    public void ManageProgress(int index)
    {
        requirementsList[index] = !requirementsList[index];
        progressIndicators[index].GetComponent<MeshRenderer>().material = requirementsList[index] ? materials[1] : materials[0];

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
