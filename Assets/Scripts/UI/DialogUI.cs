using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool optionsShown;
    public float typingSpeed = 0.02f;

    private NodeSaveData currentNode;
    private Action<ChoiceSaveData> choiceCallback;

    public void UpdateDialog(NodeSaveData node, string npcName = "")
    {
        currentNode = node;
        optionsShown = false;
        choiceCallback = null;

        _npcPreview.headImage.transform.parent.parent.localPosition = new Vector3(_npcPreview.headImage.transform.parent.parent.localPosition.x, 150, 0);
        _playerPreview.headImage.transform.parent.parent.localPosition = new Vector3(_playerPreview.headImage.transform.parent.parent.localPosition.x, 100, 0);

        dialogText.text = "";
        dialogText.maxVisibleCharacters = 0;

        ClearChoices();

        dialogText.gameObject.SetActive(true);

        StartCoroutine(TextWriting($"{npcName} - ", currentNode.text));
    }

    public void ShowChoices(List<ChoiceSaveData> choices, Action<ChoiceSaveData> onSelected)
    {
        optionsShown = true;
        choiceCallback = onSelected;
        dialogText.gameObject.SetActive(false);

        _npcPreview.headImage.transform.parent.parent.localPosition = new Vector3(_npcPreview.headImage.transform.parent.parent.localPosition.x, 100, 0);
        _playerPreview.headImage.transform.parent.parent.localPosition = new Vector3(_playerPreview.headImage.transform.parent.parent.localPosition.x, 150, 0);

        ClearChoices();

        foreach (ChoiceSaveData choice in choices)
        {
            Button button = Instantiate(optionBTN, parent);
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = choice.text;
            button.onClick.AddListener(() =>
            {
                ClearChoices();
                choiceCallback?.Invoke(choice);
            });
        }
    }

    public void OnDialogClick()
    {
        if (!finishedSpelling)
        {
            SpeedUpDialog();
            return;
        }

        if (optionsShown)
            return;

        DialogController.instance.ContinueDialog();
    }

    private void ClearChoices()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    public void SpeedUpDialog()
    {
        if (dialogText.textInfo != null)
        {
            dialogText.maxVisibleCharacters =
                dialogText.textInfo.characterCount;
        }

        finishedSpelling = true;
    }

    private IEnumerator TextWriting(string npcName, string dialog)
    {
        finishedSpelling = false;

        dialogText.text = $"<color=yellow>{npcName}</color>{dialog}";
        dialogText.ForceMeshUpdate();

        int totalCharacters = dialogText.textInfo.characterCount;
        dialogText.maxVisibleCharacters = npcName.Length;

        for (int i = npcName.Length; i <= totalCharacters; i++)
        {
            if (finishedSpelling)
                yield break;

            dialogText.maxVisibleCharacters = i;

            yield return new WaitForSeconds(typingSpeed);
        }

        finishedSpelling = true;
    }
}