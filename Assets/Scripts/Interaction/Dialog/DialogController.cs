using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [SerializeField] private DialogData _dialogData;
    [SerializeField] private Dialog _currentDialog;
    private int dialogIndex;
    public PlayerPreview _playerPreview;
    public PlayerPreview _npcPreview;

    public TMP_Text dialogText;
    public List<Button> buttons = new();

    private void Awake()
    {
        instance = this;
        StartDialog(_dialogData);
    }

    public void StartDialog(DialogData _dialogData)
    {
        this._dialogData = _dialogData;
        dialogIndex = 0;
        _currentDialog = this._dialogData.dialogs[0];
        UpdateDialog(_currentDialog);
    }

    private void UpdateDialog(Dialog dialog)
    {
        dialogText.text = dialog.text;

        //Delete listeners
        foreach (Button button in buttons)
        {
            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        //Add listeners
        for (int i = 0; i < dialog.responses.Count; i++)
        {
            if (i < buttons.Count)
            {
                //Activate buttons that are needed
                buttons[i].gameObject.SetActive(true); 
                buttons[i].transform.GetChild(0).GetComponent<TMP_Text>().text = dialog.responses[i].text;

                //Make respond to dialog
                buttons[i].onClick.AddListener(() =>
                {
                    //Checks if starts next dialog
                    if (i < dialog.responses.Count)
                    {
                        Dialog nextDialog = dialog.responses[i].nextDialog;
                        if (nextDialog != null)
                        {
                            _currentDialog = nextDialog;
                            UpdateDialog(_currentDialog);
                        }
                    }
                });
            }
        }
    }
}
