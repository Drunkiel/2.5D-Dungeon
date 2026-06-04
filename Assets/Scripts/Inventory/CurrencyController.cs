using TMPro;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController instance;

    private int lumens; //In game currency
    [SerializeField] private TMP_Text currencyDisplayText;

    void Awake()
    {
        instance = this;
    }

    public int GetLumens() => lumens;

    public void GiveLumens(int value)
    {
        lumens += value;
        currencyDisplayText.text = $"{lumens}";
    }

    public bool TakeLumens(int value)
    {
        if (lumens < value)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Not enough lumens: {value - lumens}");
            return false;
        }

        lumens -= value;
        currencyDisplayText.text = $"{lumens}";
        return true;
    }
}
