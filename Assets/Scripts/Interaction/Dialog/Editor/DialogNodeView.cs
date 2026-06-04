using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogNodeView : Node
{
    public string GUID;

    public NodeTypes NodeType;

    public Port Input;
    public Port Output;

    public IntegerField EntityIDField;
    public TextField DialogueTextField;
    public ObjectField ActionField;
    public List<ChoicePortView> ChoicePorts = new();
    public ObjectField ConditionField;
    public Port TrueOutput;
    public Port FalseOutput;

    public DialogNodeView(NodeTypes type)
    {
        GUID = Guid.NewGuid().ToString();

        NodeType = type;

        title = $"{type} Node";

        CreatePorts();

        Draw();

        SetNodeColor();

        RefreshExpandedState();
        RefreshPorts();
    }

    private void CreatePorts()
    {
        switch (NodeType)
        {
            case NodeTypes.Start:
                Output = GeneratePort(
                    "Output",
                    Direction.Output,
                    Port.Capacity.Single);

                outputContainer.Add(Output);
                break;

            case NodeTypes.Dialogue:
                Input = GeneratePort(
                    "Input",
                    Direction.Input,
                    Port.Capacity.Multi);

                Output = GeneratePort(
                    "Output",
                    Direction.Output,
                    Port.Capacity.Single);

                inputContainer.Add(Input);
                outputContainer.Add(Output);
                break;

            case NodeTypes.Choice:
                Input = GeneratePort(
                    "Input",
                    Direction.Input,
                    Port.Capacity.Single);

                inputContainer.Add(Input);
                break;

            case NodeTypes.Event:
                Input = GeneratePort(
                    "Input",
                    Direction.Input,
                    Port.Capacity.Multi);

                Output = GeneratePort(
                    "Output",
                    Direction.Output,
                    Port.Capacity.Single);

                inputContainer.Add(Input);
                outputContainer.Add(Output);
                break;

            case NodeTypes.If:
                Input = GeneratePort(
                    "Input",
                    Direction.Input,
                    Port.Capacity.Multi);

                TrueOutput = GeneratePort(
                    "True",
                    Direction.Output,
                    Port.Capacity.Single);

                FalseOutput = GeneratePort(
                    "False",
                    Direction.Output,
                    Port.Capacity.Single);

                inputContainer.Add(Input);
                outputContainer.Add(TrueOutput);
                outputContainer.Add(FalseOutput);
                break;

            case NodeTypes.End:
                Input = GeneratePort(
                    "Input",
                    Direction.Input,
                    Port.Capacity.Multi);

                inputContainer.Add(Input);
                break;
        }
    }

    private void Draw()
    {
        switch (NodeType)
        {
            case NodeTypes.Dialogue:
                DrawDialogueNode();
                break;

            case NodeTypes.Choice:
                DrawChoiceNode();
                break;

            case NodeTypes.Event:
                DrawEventNode();
                break;

            case NodeTypes.If:
                DrawIfNode();
                break;
        }
    }

    private void DrawDialogueNode()
    {
        EntityIDField = new IntegerField("Entity ID");

        DialogueTextField = new TextField("Message")
        {
            multiline = true
        };

        DialogueTextField.style.minWidth = 300;

        EntityIDField.RegisterValueChangedCallback(_ =>
        {
            SaveGraph();
        });

        DialogueTextField.RegisterValueChangedCallback(_ =>
        {
            SaveGraph();
        });

        extensionContainer.Add(EntityIDField);
        extensionContainer.Add(DialogueTextField);
    }

    private void DrawChoiceNode()
    {
        Button addChoiceButton = new(() =>
            {
                AddChoicePort("Choice");
            })
        {
            text = "Add Choice"
        };

        extensionContainer.Add(addChoiceButton);
    }

    public void AddChoicePort(string defaultText)
    {
        string guid = Guid.NewGuid().ToString();

        VisualElement row = new();

        row.style.flexDirection = FlexDirection.Row;
        row.style.marginTop = 4;

        Button deleteButton = new()
        {
            text = "X"
        };

        deleteButton.style.width = 30;
        deleteButton.style.marginRight = 4;

        TextField textField = new()
        {
            value = defaultText
        };

        textField.style.flexGrow = 1;
        textField.style.minWidth = 194;

        textField.RegisterValueChangedCallback(_ =>
        {
            SaveGraph();
        });

        //Port
        Port output = GeneratePort("Output", Direction.Output, Port.Capacity.Single);

        //Remove functionality
        deleteButton.clicked += () => RemoveChoicePort(output, row);

        //Layout
        row.Add(deleteButton);
        row.Add(textField);
        row.Add(output);
        extensionContainer.Add(row);

        //Save graph data
        ChoicePorts.Add(new ChoicePortView()
        {
            GUID = guid,
            Port = output,
            TextField = textField
        });

        RefreshPorts();
        RefreshExpandedState();
        SaveGraph();
    }

    private void RemoveChoicePort(Port port, VisualElement row)
    {
        List<Edge> edges = port.connections.ToList();

        foreach (Edge edge in edges)
        {
            edge.input.Disconnect(edge);
            edge.output.Disconnect(edge);
            edge.parent.Remove(edge);
        }

        extensionContainer.Remove(row);
        ChoicePorts.RemoveAll(x => x.Port == port);

        RefreshPorts();
        RefreshExpandedState();
        SaveGraph();
    }

    private void DrawEventNode()
    {
        ActionField = new ObjectField("Event")
        {
            objectType = typeof(DialogEvent),
            allowSceneObjects = false
        };

        ActionField.style.minWidth = 250;
        ActionField.RegisterValueChangedCallback(_ =>
        {
            SaveGraph();
        });

        extensionContainer.Add(ActionField);
    }

    private void DrawIfNode()
    {
        ConditionField = new ObjectField("Condition")
        {
            objectType = typeof(DialogCondition),
            allowSceneObjects = false
        };

        ConditionField.style.minWidth = 250;

        ConditionField.RegisterValueChangedCallback(_ =>
        {
            SaveGraph();
        });

        extensionContainer.Add(ConditionField);
    }

    private Port GeneratePort(
        string portName,
        Direction direction,
        Port.Capacity capacity)
    {
        Port port = InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(bool));
        port.portName = portName;

        return port;
    }

    private void SetNodeColor()
    {
        //Set background color
        Color color;
        switch (NodeType)
        {
            case NodeTypes.Start:
                ColorUtility.TryParseHtmlString(
                    "#769A72",
                    out color);
                break;

            case NodeTypes.Dialogue:
                ColorUtility.TryParseHtmlString(
                    "#227C9B",
                    out color);
                break;

            case NodeTypes.Choice:
                ColorUtility.TryParseHtmlString(
                    "#D6A94F",
                    out color);
                break;

            case NodeTypes.Event:
                ColorUtility.TryParseHtmlString(
                    "#D2BC52",
                    out color);
                break;

            case NodeTypes.If:
                ColorUtility.TryParseHtmlString(
                    "#8E6CD1",
                    out color);
                break;

            case NodeTypes.End:
                ColorUtility.TryParseHtmlString(
                    "#DA5561",
                    out color);
                break;

            default:
                color = Color.gray;
                break;
        }

        titleContainer.style.backgroundColor = color;

        //Set label Title color
        Label titleLabel = titleContainer.Q<Label>();

        if (titleLabel != null)
            titleLabel.style.color = Color.black;
    }

    private void SaveGraph()
    {
        DialogGraphWindow window = EditorWindow.GetWindow<DialogGraphWindow>();

        if (window == null)
            return;

        if (window.IsLoading)
            return;

        window.SaveGraph();
    }
}