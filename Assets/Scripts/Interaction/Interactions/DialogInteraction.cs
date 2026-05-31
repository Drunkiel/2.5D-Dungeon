using UnityEngine;

public class DialogInteraction : MonoBehaviour
{
    public DialogGraph dialogIndex;

    public void StartDialog()
    {
        DialogController.instance._graph = dialogIndex;
        DialogController.instance.StartDialog();
    }
}
