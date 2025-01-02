using UnityEngine;

public class DialogInteraction : MonoBehaviour
{
    public int dialogIndex;

    public void StartDialog()
    {
        if (TryGetComponent(out EnemyController _enemyController))
            DialogController.instance.StartDialog(dialogIndex, _enemyController);
        else
            DialogController.instance.StartDialog(dialogIndex);
    }
}
