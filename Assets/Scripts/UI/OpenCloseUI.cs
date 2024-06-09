using UnityEngine;

public class OpenCloseUI : MonoBehaviour
{
    private bool isOpen;
    public GameObject UI;

    public void OpenClose()
    {
        isOpen = !isOpen;
        UI.SetActive(isOpen);
    }

    public void Close()
    {
        isOpen = false;
        UI.SetActive(false);
    }
}
