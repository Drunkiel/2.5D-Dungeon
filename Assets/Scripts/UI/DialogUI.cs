using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    public TMP_Text dialogText;
    public EntityPreview _npcPreview;
    public EntityPreview _playerPreview;
    public Transform parent;
    public Button optionBTN;

    public bool finishedSpelling;
    private bool optionsShown = false;
    public float typingSpeed = 0.02f;
    private Dialog _currentDialog;

    public void UpdateDialog(Dialog _dialog)
    {
        _currentDialog = _dialog;
        optionsShown = false;

        //!TEST OF CONCEPT
        _npcPreview.headImage.transform.parent.parent.localPosition = new(_npcPreview.headImage.transform.parent.parent.localPosition.x, 150, 0);
        _playerPreview.headImage.transform.parent.parent.localPosition = new(_playerPreview.headImage.transform.parent.parent.localPosition.x, 100, 0);

        //Reset text
        dialogText.text = "";
        dialogText.maxVisibleCharacters = 0;

        //Delete all response Buttons
        for (int i = 0; i < parent.childCount; i++)
            Destroy(parent.GetChild(i).gameObject);

        dialogText.gameObject.SetActive(true);

        StartCoroutine(TextWriting(_dialog.text));
    }

    public void OnDialogClick()
    {
        if (!finishedSpelling)
        {
            SpeedUpDialog();
            return;
        }

        if (!optionsShown)
        {
            ShowResponseOptions();
            return;
        }
    }

    private void ShowResponseOptions()
    {
        optionsShown = true;
        dialogText.gameObject.SetActive(false);

        //!TEST OF CONCEPT
        _npcPreview.headImage.transform.parent.parent.localPosition = new(_npcPreview.headImage.transform.parent.parent.localPosition.x, 100, 0);
        _playerPreview.headImage.transform.parent.parent.localPosition = new(_playerPreview.headImage.transform.parent.parent.localPosition.x, 150, 0);

        //Create response Buttons
        for (int i = 0; i < _currentDialog._responseOptions.Count; i++)
        {
            Button newOptionButton = Instantiate(optionBTN, parent);
            int a = i;

            newOptionButton.onClick.AddListener(() => _currentDialog._responseOptions[a].actionToDo.Invoke());

            newOptionButton.transform.GetChild(0).GetComponent<TMP_Text>().text =
                _currentDialog._responseOptions[i].text;
        }
    }

    public void SpeedUpDialog()
    {
        if (dialogText.textInfo != null)
            dialogText.maxVisibleCharacters = dialogText.textInfo.characterCount;
        finishedSpelling = true;
    }

    IEnumerator TextWriting(string dialog)
    {
        finishedSpelling = false;

        dialogText.text = dialog;
        dialogText.ForceMeshUpdate();

        int totalCharacters = dialogText.textInfo.characterCount;

        dialogText.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalCharacters; i++)
        {
            if (finishedSpelling)
                yield break;

            dialogText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        finishedSpelling = true;
    }
}