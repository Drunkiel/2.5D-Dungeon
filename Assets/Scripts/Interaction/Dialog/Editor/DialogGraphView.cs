using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphView : GraphView
{
    public DialogGraphView()
    {
        style.flexGrow = 1;

        Insert(0, new GridBackground());

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddStartNode();
        graphViewChanged += OnGraphChanged;
    }

    private GraphViewChange OnGraphChanged(GraphViewChange change)
    {
        DialogGraphWindow window = EditorWindow.GetWindow<DialogGraphWindow>();

        if (window != null && !window.IsLoading)
            window.SaveGraph();

        return change;
    }

    private void AddStartNode()
    {
        DialogNodeView startNode = new(NodeTypes.Start);
        startNode.SetPosition(new Rect(100, 300, 250, 150));
        AddElement(startNode);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList()
            .Where(port =>
            {
                if (port == startPort)
                    return false;

                if (port.node == startPort.node)
                    return false;

                if (port.direction ==
                    startPort.direction)
                    return false;

                return true;
            })
            .ToList();
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 mousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);

        evt.menu.AppendAction(
            "Add Dialogue Node",
            x => CreateNode(
                NodeTypes.Dialogue,
                mousePosition));

        evt.menu.AppendAction(
            "Add Choice Node",
            x => CreateNode(
                NodeTypes.Choice,
                mousePosition));

        evt.menu.AppendAction(
            "Add Event Node",
            x => CreateNode(
                NodeTypes.Event,
                mousePosition));

        evt.menu.AppendAction(
            "Add End Node",
            x => CreateNode(
                NodeTypes.End,
                mousePosition));
    }

    private void CreateNode(NodeTypes type, Vector2 position)
    {
        DialogNodeView node = new(type);
        node.SetPosition(new Rect(position, new Vector2(250, 200)));
        AddElement(node);
    }
}