using UnityEngine;

[System.Serializable]
public class OptionTab
{
    public GameObject optionTab;
    public GameObject graphicsOptions;
    public GameObject audioOptions;
    public GameObject controlOptions;

    public void OpenTab(int value)
    {
        optionTab.SetActive(true);

        switch (value)
        {
            case 0:
                graphicsOptions.SetActive(true);
                audioOptions.SetActive(false);
                controlOptions.SetActive(false);
                break;

            case 1:
                graphicsOptions.SetActive(false);
                audioOptions.SetActive(true);
                controlOptions.SetActive(false);
                break;

            case 2:
                graphicsOptions.SetActive(false);
                audioOptions.SetActive(false);
                controlOptions.SetActive(true);
                break;
        }
    }
}

public class PauseMenu : MonoBehaviour
{
    public GameObject selectionMenu;
    public GameObject optionsMenu;
    public OptionTab _optionTab;
    public GameObject creditsTab;

    public void ResetPauseMenu()
    {
        selectionMenu.SetActive(true);
        optionsMenu.SetActive(false);
        _optionTab.optionTab.SetActive(false);
        _optionTab.graphicsOptions.SetActive(false);
        _optionTab.audioOptions.SetActive(false);
        _optionTab.controlOptions.SetActive(false);
        creditsTab.SetActive(false);
    }

    public void OpenOptions()
    {
        selectionMenu.SetActive(false);
        optionsMenu.SetActive(true);
        creditsTab.SetActive(false);
    }

    public void OpenCredits()
    {
        creditsTab.SetActive(true);
    }

    public void SelectOptionTab(int value)
    {
        _optionTab.OpenTab(value);
    }
}
