using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [SerializeField] private DialogData _dialogData;
    private int dialogIndex;

    public TMP_Text dialogText;
    public PlayerPreview _npcPreview;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void StartDialog(DialogData _dialogData)
    {
        this._dialogData = _dialogData;
        dialogIndex = 0;
        _openCloseUI.Open();
        UpdateDialog();
    }

    public void EndDialog()
    {
        _openCloseUI.Close();
    }

    private void UpdateDialog()
    {
        dialogText.text = _dialogData.dialogs[dialogIndex];
    }

    public void NextDialog()
    {
        dialogIndex++;
        if (dialogIndex < _dialogData.dialogs.Count)
            UpdateDialog();
        else
            EndDialog();
    }
}
