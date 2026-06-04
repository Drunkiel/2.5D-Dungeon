using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphWindow : EditorWindow
{
    private DialogGraphView graphView;
    private DialogGraph currentGraph;

    public bool IsLoading { get; private set; }

    [MenuItem("Tools/Dialog Graph")]
    public static void Open()
    {
        DialogGraphWindow wnd = GetWindow<DialogGraphWindow>();
        wnd.titleContent = new GUIContent("Dialog Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new DialogGraphView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    public static void Open(DialogGraph graph)
    {
        DialogGraphWindow window = GetWindow<DialogGraphWindow>();

        window.titleContent = new GUIContent("Dialogue Graph");
        window.currentGraph = graph;
        window.LoadGraph();
    }

    public void SaveGraph()
    {
        if (currentGraph == null)
            return;

        currentGraph.nodes.Clear();
        currentGraph.edges.Clear();

        foreach (DialogNodeView node in graphView.nodes.Cast<DialogNodeView>())
        {
            NodeSaveData data = new()
            {
                GUID = node.GUID,
                nodeType = node.NodeType,
                position = node.GetPosition().position
            };

            switch (node.NodeType)
            {
                case NodeTypes.Dialogue:
                    data.entityID = (short)node.EntityIDField.value;
                    data.text = node.DialogueTextField.value;
                    break;

                case NodeTypes.Choice:
                    foreach (ChoicePortView choice in node.ChoicePorts)
                    {
                        data.choices.Add(new ChoiceSaveData()
                        {
                            GUID = choice.GUID,
                            text = choice.TextField.value
                        });
                    }
                    break;

                case NodeTypes.Event:
                    data.action = node.ActionField.value as DialogEvent;
                    break;

                case NodeTypes.If:
                    data.condition = node.ConditionField.value as DialogCondition;
                    break;
            }

            currentGraph.nodes.Add(data);
        }

        foreach (Edge edge in graphView.edges)
        {
            DialogNodeView outputNode = edge.output.node as DialogNodeView;
            DialogNodeView inputNode = edge.input.node as DialogNodeView;

            EdgeSaveData edgeData = new()
            {
                outputNodeGUID = outputNode.GUID,
                inputNodeGUID = inputNode.GUID,
                outputPortName = edge.output.portName
            };

            ChoicePortView choice = outputNode.ChoicePorts.Find(x => x.Port == edge.output);

            if (choice != null)
                edgeData.outputPortGUID = choice.GUID;

            currentGraph.edges.Add(edgeData);
        }

        EditorUtility.SetDirty(currentGraph);
        AssetDatabase.SaveAssets();
    }

    private void LoadGraph()
    {
        if (currentGraph == null)
            return;

        IsLoading = true;

        try
        {
            graphView.graphElements
                .ToList()
                .ForEach(element =>
                {
                    graphView.RemoveElement(element);
                });

            Dictionary<string, DialogNodeView> nodeLookup = new();

            //Load Nodes
            foreach (NodeSaveData nodeData in currentGraph.nodes)
            {
                DialogNodeView node = new(nodeData.nodeType)
                {
                    GUID = nodeData.GUID
                };

                node.SetPosition(new Rect(nodeData.position, new Vector2(250, 200)));

                switch (nodeData.nodeType)
                {
                    case NodeTypes.Dialogue:
                        node.EntityIDField.value = nodeData.entityID;
                        node.DialogueTextField.value = nodeData.text;
                        break;

                    case NodeTypes.Choice:
                        foreach (ChoiceSaveData choice in nodeData.choices)
                        {
                            node.AddChoicePort(choice.text);
                            ChoicePortView createdPort = node.ChoicePorts.Last();
                            createdPort.GUID = choice.GUID;
                        }
                        break;

                    case NodeTypes.Event:
                        node.ActionField.value = nodeData.action;
                        break;

                    case NodeTypes.If:
                        node.ConditionField.value = nodeData.condition;
                        break;
                }

                graphView.AddElement(node);
                nodeLookup.Add(node.GUID, node);
            }

            //Load connections between nodes
            foreach (EdgeSaveData edgeData in currentGraph.edges)
            {
                if (!nodeLookup.TryGetValue(edgeData.outputNodeGUID, out DialogNodeView outputNode))
                    continue;

                if (!nodeLookup.TryGetValue(edgeData.inputNodeGUID, out DialogNodeView inputNode))
                    continue;

                Port outputPort = null;

                if (outputNode.NodeType == NodeTypes.Choice)
                {
                    ChoicePortView choice =
                        outputNode.ChoicePorts.FirstOrDefault(
                            x => x.GUID == edgeData.outputPortGUID);

                    if (choice != null)
                        outputPort = choice.Port;
                }
                else if (outputNode.NodeType == NodeTypes.If)
                {
                    if (edgeData.outputPortName == "True")
                        outputPort = outputNode.TrueOutput;
                    else if (edgeData.outputPortName == "False")
                        outputPort = outputNode.FalseOutput;
                }
                else
                {
                    outputPort = outputNode.Output;
                }

                Port inputPort = inputNode.Input;

                if (outputPort == null || inputPort == null)
                    continue;

                Edge edge = outputPort.ConnectTo(inputPort);
                graphView.AddElement(edge);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}