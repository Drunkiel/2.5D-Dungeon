using UnityEngine;

public class DialogInteraction : MonoBehaviour
{
    public DialogData _dialogData;

    public void StartDialog()
    {
        if (TryGetComponent(out EnemyController _enemyController))
            DialogController.instance.StartDialog(_dialogData, _enemyController);
        else
            DialogController.instance.StartDialog(_dialogData);
    }
}
