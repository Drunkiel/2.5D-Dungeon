using TMPro;
using UnityEngine;

public class ConsoleCommands : MonoBehaviour
{
    [SerializeField] private ConsoleController _consoleController;

    public void Clear(int a)
    {
        foreach (TMP_Text singleMessage in _consoleController.messages)
            Destroy(singleMessage.gameObject);
        _consoleController.messages.Clear();
    }
}
