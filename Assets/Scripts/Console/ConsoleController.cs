using System;
using System.Collections.Generic;
using System.Globalization;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash) && !chatInput.isFocused)
            GetComponent<OpenCloseUI>().OpenClose();
    }

    public void SendToChat()
    {
        //Check if message is empty
        if (string.IsNullOrEmpty(chatInput.text))
            return;

        if (chatInput.text[0] != '/')
        {
            ChatMessage(SenderType.Player, chatInput.text);
        }
        else
        {
            commandName = GetCommand();
            commandName = char.ToUpper(commandName[0]) + commandName[1..];

            try
            {
                SendCommand(commandName, commandAttributes.ToArray());
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

        //Delete messages
        if (messages.Count > 20)
        {
            Destroy(messages[0].gameObject);
            messages.RemoveAt(0);
        }

        chatInput.text = "";
        chatInput.ActivateInputField();
    }

    private void SendCommand(string commandName, params string[] parameters)
    {
        Type consoleCommandsType = typeof(ConsoleCommands);
        MethodInfo method = consoleCommandsType.GetMethod(commandName);

        ParameterInfo[] methodParams = method.GetParameters();

        if (methodParams.Length == parameters.Length)
        {
            object[] convertedParams = new object[parameters.Length];
            bool allMatch = true;

            for (int i = 0; i < methodParams.Length; i++)
            {
                try
                {
                    convertedParams[i] = Convert.ChangeType(parameters[i], methodParams[i].ParameterType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                method.Invoke(_commands, convertedParams);
                return;
            }
        }

        ChatMessage(SenderType.System, $"No matching command found for: {commandName} with {parameters.Length} arguments", OutputType.Error);
    }

    public void ChatMessage(SenderType sender, string message, OutputType outputType = OutputType.Normal)
    {
        TMP_Text newText = Instantiate(chatTextPrefab, chatParent);

        string senderName = (int)sender != 2 ? sender.ToString() : string.Empty;
        newText.text = $"<color=yellow><b>{senderName}:</b></color> {message}";

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
        string[] splittedString = chatInput.text[1..].Split(' ');
        commandAttributes.Clear();
        commandAttributes.AddRange(splittedString[1..]);

        return splittedString[0];
    }
}
