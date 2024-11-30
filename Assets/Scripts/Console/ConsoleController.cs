using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public static ConsoleController instance;

    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatParent;
    [SerializeField] private ScrollRect scrollRect;
    public List<TMP_Text> messages = new();
    public List<string> previousMessages = new();
    private int previousMessagesIndex;
    [SerializeField] private string commandName = "";
    [SerializeField] private List<string> commandAttributes = new();

    [SerializeField] private TMP_Text chatTextPrefab;
    [SerializeField] private ConsoleCommands _commands;

    private void Awake()
    {
        instance = this;
    }

    public void ManageChat(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!chatInput.isFocused && !GameController.isPaused)
            GetComponent<OpenCloseUI>().OpenClose();
    }

    public void IndexDownMessage(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (previousMessages.Count == 0 || GameController.isPaused)
            return;

        previousMessagesIndex++;
        if (previousMessagesIndex >= previousMessages.Count)
            previousMessagesIndex = previousMessages.Count - 1;
            
        chatInput.text = previousMessages[previousMessagesIndex];
    }

    public void IndexUpMessage(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (previousMessages.Count == 0 || GameController.isPaused)
            return;

        previousMessagesIndex--;
        if (previousMessagesIndex < 0)
            previousMessagesIndex = 0;
            
        chatInput.text = previousMessages[previousMessagesIndex];
    }

    public void SendToChat()
    {
        //Check if message is empty
        if (string.IsNullOrEmpty(chatInput.text))
            return;

        //Add new message to previous messages
        previousMessages.Add(chatInput.text);
        previousMessagesIndex = previousMessages.Count - 1;

        //Check if message is command or text
        if (chatInput.text[0] != '/')
            ChatMessage(SenderType.Player, chatInput.text);
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

        scrollRect.verticalNormalizedPosition = -1;
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
        previousMessagesIndex = previousMessages.Count - 1;
    }

    public void ExitChatMode()
    {
        if (!PlayerController.instance.GetComponent<EntityCombat>().inCombat)
            PlayerController.instance.isStopped = false;
    }

    private string GetCommand()
    {
        string input = chatInput.text[1..]; 
        List<string> args = new();
        bool insideQuotes = false;
        string currentArg = "";

        //Here is the check if command contains more complex string
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == '"')
            {
                insideQuotes = !insideQuotes;
                if (!insideQuotes && currentArg.Length > 0)
                {
                    args.Add(currentArg);
                    currentArg = "";
                }
            }
            else if (c == ' ' && !insideQuotes)
            {
                if (currentArg.Length > 0)
                {
                    args.Add(currentArg);
                    currentArg = "";
                }
            }
            else
                currentArg += c;
        }

        //Here add all args
        if (currentArg.Length > 0)
            args.Add(currentArg);

        commandAttributes.Clear();
        commandAttributes.AddRange(args.Skip(1));

        return args[0];
    }
}
