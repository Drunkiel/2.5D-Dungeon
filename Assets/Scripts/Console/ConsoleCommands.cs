using TMPro;
using UnityEngine;

public class ConsoleCommands : MonoBehaviour
{
    [SerializeField] private ConsoleController _consoleController;

    public void Clear()
    {
        foreach (TMP_Text singleMessage in _consoleController.messages)
            Destroy(singleMessage.gameObject);
        _consoleController.messages.Clear();
    }

    public void Say(string text)
    {
        _consoleController.ChatMessage(SenderType.System, text);
    }

    public void ReloadItems()
    {
        ItemContainer.instance.UnLoadStuff();
        ItemContainer.instance.LoadStuff();
    }

    public void ReloadStatistics()
    {
        PlayerController _playerController = PlayerController.instance;

        _playerController._statistics.ResetStatistics();
        _playerController._statistics.RecalculateStatistics(_playerController._holdingController._itemController._gearHolder);
    }

    public void GetItem(string itemName)
    {
        ItemContainer.instance.GetItemByName(itemName);
    }

    public void KillEvent(string id)
    {
        if (!short.TryParse(id, out short targetID))
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"'{id}' can't be parsed to number", OutputType.Error);
            return;
        }

        QuestController.instance.InvokeKillEvent(targetID);
    }
}
