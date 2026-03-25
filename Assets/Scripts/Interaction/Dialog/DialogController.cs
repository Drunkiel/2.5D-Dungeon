using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [SerializeField] private List<Dialog> _dialogs = new();
    private int dialogIndex;
    public bool isTalking;

    public DialogUI _dialogUI;
    private EntityController _entityController;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialog(int index)
    {
        EntityController _playerController = GameController.instance._player;

        if (isTalking)
        {
            _dialogUI.SpeedUpDialog();
            return;
        }

        //Assigning values
        dialogIndex = index;
        _entityController = EntityHolder.instance.GetEntityInScene(_dialogs[dialogIndex].entityID);

        if (_entityController != null)
        {
            UpdatePreviewLook(_entityController, _dialogUI._npcPreview);
            UpdatePreviewLook(_playerController, _dialogUI._playerPreview);

            //Checking if player is talking to right npc
            QuestController.instance.InvokeTalkEvent(_entityController._entityInfo.ID);
        }
        else
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Entity {_dialogs[dialogIndex].entityID} does not exists", OutputType.Error);
            return;
        }

        _openCloseUI.Open();
        _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
        isTalking = true;
    }

    private void UpdatePreviewLook(EntityController _entityController, EntityPreview _entityPreview)
    {
        _entityController.StopEntity(true);
        EntityLookController _entityLookController = _entityController.GetComponent<EntityLookController>();
        _entityPreview.UpdateAllByEntity(_entityLookController._entityLook, _entityLookController._spriteHolder, _entityController._holdingController._itemController._gearHolder);
    }

    public void ChangeDialog(int index)
    {
        if (!_dialogUI.finishedSpelling)
        {
            _dialogUI.SpeedUpDialog();
            return;
        }

        dialogIndex = index;
        _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
    }

    public void ForceChangeDialog(int index)
    {
        if (!_dialogUI.finishedSpelling)
            _dialogUI.SpeedUpDialog();

        dialogIndex = index;
        _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
    }

    public void EndDialog()
    {
        if (!_dialogUI.finishedSpelling)
        {
            _dialogUI.SpeedUpDialog();
            return;
        }

        GameController.instance._player.StopEntity(false);
        if (_entityController != null)
            _entityController.StopEntity(false);

        _openCloseUI.Close();
        isTalking = false;
    }
}
