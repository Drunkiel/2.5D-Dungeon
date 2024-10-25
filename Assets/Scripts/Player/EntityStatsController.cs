using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityStatsController : MonoBehaviour
{
    public Slider healthSlider;
    private float previousHealthStatus;
    public Slider manaSlider;
    private float previousManaStatus;
    [SerializeField] private Transform displayParent;
    [SerializeField] private GameObject statusDisplayObject;

    public void UpdateHealthSlider(float newValue)
    {
        if (newValue == 1 || newValue == previousHealthStatus)
            return;

        TMP_Text statusDisplayText = Instantiate(statusDisplayObject, displayParent).GetComponent<TMP_Text>();

        string stringOperator()
        {
            if (previousHealthStatus > newValue)
                return "-";
            else
                return "+";
        }

        statusDisplayText.text = $"{stringOperator()}{Mathf.Round((previousHealthStatus - newValue) * 100)}HP";

        previousHealthStatus = newValue;
    }

    public void UpdateManaSlider(float newValue)
    {
        if (newValue == 1 || newValue == previousHealthStatus)
            return;

        TMP_Text statusDisplayText = Instantiate(statusDisplayObject, displayParent).GetComponent<TMP_Text>();

        string stringOperator()
        {
            if (previousManaStatus > newValue)
                return "-";
            else
                return "+";
        }

        statusDisplayText.text = $"{stringOperator()}{Mathf.Round((previousManaStatus - newValue) * 100)}MN";

        previousManaStatus = newValue;
    }
}
