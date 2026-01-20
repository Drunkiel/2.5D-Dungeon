using TMPro;
using UnityEngine;

public class HintController : MonoBehaviour
{
    public static HintController instance;
    private int currentHint;
    public string[] hintTexts;

    [SerializeField] private TMP_Text hintText;
    [SerializeField] private GameObject hintObject;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateText(int hintIndex)
    {
        if (hintIndex == currentHint) 
            return;

        currentHint = hintIndex;

        if (hintIndex == 0)
            hintObject.SetActive(false);
        else
            hintObject.SetActive(true);

        hintText.text = hintTexts[hintIndex];
    }
}
