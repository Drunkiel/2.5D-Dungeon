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
            ConsoleController.instance.ChatMessage(SenderType.System, $"'<color=yellow>{id}</color>' can't be parsed to number", OutputType.Error);
            return;
        }

        QuestController.instance.InvokeKillEvent(targetID);
    }

    public void PopUp(string text)
    {
        PopUpController.instance.CreatePopUp(PopUpInfo.QuestCompleted, text);
    }

    //Teleport
    public void Tp(string sceneName)
    {
        PortalController.instance.TeleportToScene(sceneName, Vector3.up * 10);
    }

    public void Debug()
    {
        transform.parent.GetComponent<OpenCloseUI>().OpenClose();
    }

    public void Skin(string newPath, string bodyType)
    {
        PlayerController.instance.GetComponent<EntityLookController>().SpriteLoader(newPath, bodyType);
    }

    public void SkinCut(string skinPath, string outputFolder)
    {
        ImageCutter _imageCutter = GetComponent<ImageCutter>();
        _imageCutter.sourceImage = SaveLoadSystem.GetSprite(SaveLoadSystem.skinsSavePath + skinPath, 20f).texture;
        _imageCutter.ExtractImages(outputFolder);
    }
}
