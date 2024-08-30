using UnityEngine;

public class DialogInteraction : MonoBehaviour
{
    public DialogData _dialogData;

    public void StartDialog()
    {
        DialogController.instance.StartDialog(_dialogData);
    }
}
