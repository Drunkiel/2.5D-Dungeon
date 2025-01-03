using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [SerializeField] private List<Dialog> _dialogs = new();
    private int dialogIndex;
    private bool isTalking;

    public DialogUI _dialogUI;
    private EnemyController _enemyController;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialog(int index)
    {
        StartDialog(index, _enemyController);
    }

    public void StartDialog(int index, EnemyController _enemyController = null)
    {
        PlayerController _playerController = PlayerController.instance;

        if (isTalking)
        {
            _dialogUI.SpeedUpDialog(_dialogs[dialogIndex].text);
            return;
        }

        //Assigning values
        this._enemyController = _enemyController;
        dialogIndex = index;
        _playerController.isStopped = true;

        if (_enemyController != null)
        {
            _enemyController._enemyWalk.isStopped = true;
            _enemyController._enemyWalk.GoTo(_playerController.transform.position, _playerController.transform.position);
            _dialogUI._npcPreview.UpdateAllByEntity(_enemyController.GetComponent<EntityLookController>()._entityLook);
            _dialogUI.nameText.text = _enemyController._entityInfo.name;

            //Checking if player is talking to right npc
            QuestController.instance.InvokeTalkEvent(_enemyController._entityInfo.ID);
        }
        _openCloseUI.Open();
        _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
        isTalking = true;
    }

    public void ChangeDialog(int index)
    {
        dialogIndex = index;
        _dialogUI.UpdateDialog(_dialogs[dialogIndex]);
    }

    public void EndDialog()
    {
        PlayerController.instance.isStopped = false;
        if (_enemyController != null)
            _enemyController._enemyWalk.isStopped = false;

        _openCloseUI.Close();
        isTalking = false;
    }
}
