using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatParent;
    [SerializeField] private TMP_Text chatTextPrefab;
    public List<TMP_Text> messages = new();

    public void SendToChat(bool isPlayer = true)
    {
        //Checks if message is empty
        if (chatInput.text == "")
            return;

        //Creating new message
        TMP_Text newText = Instantiate(chatTextPrefab, chatParent);
        newText.text = isPlayer ? $"Player: {chatInput.text}" : $"Game: {chatInput.text}";
        messages.Add(newText);

        //Checks if there is too many messages and deletes them
        if (messages.Count > 20)
        {
            Destroy(messages[0].gameObject);
            messages.RemoveAt(0);
        }

        chatInput.text = "";
        chatInput.ActivateInputField();
    }

    public void EnterChatMode()
    {
        PlayerController.instance.isStopped = true;
    }

    public void ExitChatMode()
    {
        if (!CombatController.instance.inCombat)
            PlayerController.instance.isStopped = false;
    }
}
