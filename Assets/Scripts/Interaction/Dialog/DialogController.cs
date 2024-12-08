using System.Collections;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [SerializeField] private DialogData _dialogData;
    private int dialogIndex;
    private bool finishedSpelling;
    private bool isTalking;

    public TMP_Text dialogText;
    public EntityPreview _npcPreview;
    private EnemyController _enemyController;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialog(DialogData _dialogData, EnemyController _enemyController = null)
    {
        PlayerController _playerController = PlayerController.instance;

        if (isTalking)
        {
            NextDialog();
            return;
        }

        //Assigning values
        this._dialogData = _dialogData;
        this._enemyController = _enemyController;
        dialogIndex = 0;

        _playerController.isStopped = true;
        if (_enemyController != null)
        {
            _enemyController._enemyWalk.isStopped = true;
            _enemyController._enemyWalk.GoTo(_playerController.transform.position, _playerController.transform.position);
            _npcPreview.UpdateAllByEntity(_enemyController.GetComponent<EntityLookController>()._entityLook);
        }
        _openCloseUI.Open();
        UpdateDialog();
        isTalking = true;
    }

    public void EndDialog()
    {
        PlayerController.instance.isStopped = false;
        if (_enemyController != null)
            _enemyController._enemyWalk.isStopped = false;

        _dialogData.onEndDialogEvent.Invoke();
        _openCloseUI.Close();
        isTalking = false;
    }

    private void UpdateDialog()
    {
        dialogText.text = "";
        StartCoroutine(nameof(TextWriting), _dialogData.dialogs[dialogIndex]);
    }

    public void NextDialog()
    {
        if (!finishedSpelling)
        {
            dialogText.text = _dialogData.dialogs[dialogIndex];
            finishedSpelling = true;
            return;
        }

        dialogIndex++;
        if (dialogIndex < _dialogData.dialogs.Count)
            UpdateDialog();
        else
            EndDialog();
    }

    IEnumerator TextWriting(string dialog)
    {
        finishedSpelling = false;

        foreach (char singleCharacter in dialog)
        {
            if (finishedSpelling)
                break;

            dialogText.text += singleCharacter;
            yield return new WaitForSeconds(0.02f);
        }
        
        finishedSpelling = true;
    }
}
