using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public enum OutputType
{
    Normal,
    Warning,
    Error
}

public enum SenderType
{
    Player,
    System,
    Hidden
}

public class ConsoleController : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatParent;
    [SerializeField] private TMP_Text chatTextPrefab;
    [SerializeField] private ConsoleCommands _commands;
    public List<TMP_Text> messages = new();
    [SerializeField] private string commandName = "";
    [SerializeField] private List<string> commandAttributes = new();

    public void SendToChat()
    {
        //Checks if message is empty
        if (chatInput.text == "")
            return;

        if (chatInput.text[0] != '/')
            ChatMessage(SenderType.Player, chatInput.text);
        else
        {
            commandName = chatInput.text[1..];
            commandName = char.ToUpper(commandName[0]) + commandName[1..];

                SendCommand(commandName);
            try
            {
            }
            catch (TargetParameterCountException ex)
            {
                ChatMessage(SenderType.System, $"Command '{commandName}' failed: Incorrect number of parameters. Details: {ex.Message}", OutputType.Error);
            }
            catch (ArgumentException ex)
            {
                ChatMessage(SenderType.System, $"Command '{commandName}' failed: Invalid arguments provided. Details: {ex.Message}", OutputType.Error);
            }
            catch (Exception ex)
            {
                ChatMessage(SenderType.System, $"Command '{commandName}' failed: {ex.Message}", OutputType.Error);
            }
        }

        //Checks if there is too many messages and deletes them
        if (messages.Count > 20)
        {
            Destroy(messages[0].gameObject);
            messages.RemoveAt(0);
        }

        chatInput.text = "";
        chatInput.ActivateInputField();
    }

    private void SendCommand(string commandName, params object[] parameters)
    {
        Type consoleCommandsType = typeof(ConsoleCommands);
        MethodInfo methodInfo = consoleCommandsType.GetMethod(commandName);

        if (methodInfo != null)
        {
            methodInfo.Invoke(commandName, parameters);
        }
        else
            ChatMessage(SenderType.System, $"There is no such command as: {commandName}", OutputType.Error);
    }

    public void ChatMessage(SenderType sender, string message, OutputType outputType = OutputType.Normal)
    {
        //Creating new message
        TMP_Text newText = Instantiate(chatTextPrefab, chatParent);

        string senderName = "";
        if ((int)sender != 2)
            senderName = sender.ToString();

        newText.text = $"{senderName}: {message}";

        switch (outputType)
        {
            case OutputType.Warning:
                newText.color = Color.yellow;
                break;

            case OutputType.Error:
                newText.color = Color.red;
                break;
        }

        messages.Add(newText);
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

    private string GetCommand()
    {
        string[] splittedString = chatInput.text[1..].Split(" ");
        commandAttributes.Clear();
        commandAttributes.AddRange(splittedString[1..]);

        return splittedString[0];
    }
}
