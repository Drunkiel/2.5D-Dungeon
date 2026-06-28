using System.Collections;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    public DialogGraph _graph;
    private NodeSaveData currentNode;
    private short npcIdPreviewLoaded = -1;
    public bool isTalking;

    public DialogUI _dialogUI;
    private EntityController _entityController;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialog()
    {
        if (isTalking)
        {
            _dialogUI.OnDialogClick();
            return;
        }

        NodeSaveData startNode = _graph.nodes.Find(x => x.nodeType == NodeTypes.Start);

        if (startNode == null)
        {
            Debug.LogError("Start node not found.");
            return;
        }

        NodeSaveData firstNode = GetConnectedNode(startNode.GUID);

        if (firstNode == null)
        {
            Debug.LogError("Start node is not connected.");
            return;
        }

        EntityController player = GameController.instance._player;
        UpdatePreviewLook(player, _dialogUI._playerPreview);
        CameraController.instance.LockCamera(true);

        _openCloseUI.Open();
        isTalking = true;
        ProcessNode(firstNode);
    }

    public IEnumerator StartDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartDialog();
    }

    private NodeSaveData GetNode(string guid)
    {
        return _graph.nodes.Find(x => x.GUID == guid);
    }

    private NodeSaveData GetConnectedNode(string outputNodeGUID)
    {
        EdgeSaveData edge = _graph.edges.Find(x => x.outputNodeGUID == outputNodeGUID);

        if (edge == null)
            return null;

        return GetNode(edge.inputNodeGUID);
    }

    private NodeSaveData GetConnectedChoiceNode(string choicePortGUID)
    {
        EdgeSaveData edge = _graph.edges.Find(x => x.outputPortGUID == choicePortGUID);

        if (edge == null)
            return null;

        return GetNode(edge.inputNodeGUID);
    }

    private void ProcessNode(NodeSaveData node)
    {
        if (node == null)
        {
            EndDialog();
            return;
        }

        switch (node.nodeType)
        {
            case NodeTypes.Dialogue:
                ShowDialogueNode(node);
                break;

            case NodeTypes.Choice:
                ShowChoiceNode(node);
                break;

            case NodeTypes.Event:
                ExecuteEventNode(node);
                break;

            case NodeTypes.If:
                ExecuteIfNode(node);
                break;

            case NodeTypes.End:
                EndDialog();
                break;
        }
    }

    private void ShowDialogueNode(NodeSaveData node)
    {
        currentNode = node;

        _entityController = EntityHolder.instance.GetEntityInScene(node.entityID);

        if (_entityController != null)
        {
            if (npcIdPreviewLoaded != node.entityID)
            {
                UpdatePreviewLook(_entityController, _dialogUI._npcPreview);
                npcIdPreviewLoaded = node.entityID;
            }

            QuestController.instance.InvokeTalkEvent(_entityController._entityInfo.ID);
        }

        _dialogUI.UpdateDialog(node, _entityController != null ? _entityController._entityInfo.name : "");
    }

    private void ShowChoiceNode(NodeSaveData node)
    {
        currentNode = node;
        _dialogUI.ShowChoices(node.choices, OnChoiceSelected);
    }

    private void ExecuteEventNode(NodeSaveData node)
    {
        currentNode = node;

        node.action.Execute();

        NodeSaveData nextNode = GetConnectedNode(node.GUID);
        ProcessNode(nextNode);
    }

    private void OnChoiceSelected(ChoiceSaveData choice)
    {
        NodeSaveData nextNode = GetConnectedChoiceNode(choice.GUID);
        ProcessNode(nextNode);
    }

    private NodeSaveData GetIfResultNode(NodeSaveData ifNode, bool result)
    {
        EdgeSaveData edge = _graph.edges.Find(x => x.outputNodeGUID == ifNode.GUID && x.outputPortName == (result ? "True" : "False"));

        if (edge == null)
            return null;

        return GetNode(edge.inputNodeGUID);
    }

    private void ExecuteIfNode(NodeSaveData node)
    {
        bool result = node.condition != null && node.condition.CheckIfTrue();
        NodeSaveData nextNode = GetIfResultNode(node, result);
        ProcessNode(nextNode);
    }

    public void ContinueDialog()
    {
        if (!_dialogUI.finishedSpelling)
        {
            _dialogUI.SpeedUpDialog();
            return;
        }

        NodeSaveData nextNode = GetConnectedNode(currentNode.GUID);
        ProcessNode(nextNode);
    }

    private void UpdatePreviewLook(EntityController entityController, EntityPreview entityPreview)
    {
        entityController.StopEntity(true);
        EntityLookController lookController = entityController.GetComponent<EntityLookController>();

        entityPreview.UpdateAllByEntity(
            lookController._entityLook,
            lookController._spriteHolder,
            entityController._itemController._gearHolder);
    }

    public void EndDialog()
    {
        if (!_dialogUI.finishedSpelling)
        {
            _dialogUI.SpeedUpDialog();
            return;
        }

        CameraController.instance.LockCamera(false);
        GameController.instance._player.StopEntity(false);

        if (_entityController != null)
            _entityController.StopEntity(false);

        _openCloseUI.Close();
        currentNode = null;
        npcIdPreviewLoaded = -1;
        isTalking = false;
    }
}