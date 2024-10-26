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
    [SerializeField] private Transform buffsParent;
    [SerializeField] private Image buffImage;

    public void UpdateHealthSlider(float newValue)
    {
        if (newValue == 1 || newValue == previousHealthStatus)
            return;

        //Checks if first time load
        if (previousHealthStatus != 0)
        {
            TMP_Text statusDisplayText = Instantiate(statusDisplayObject, displayParent).GetComponent<TMP_Text>();

            string stringOperator()
            {
                if (previousHealthStatus > newValue)
                    return "-";
                else
                    return "+";
            }

            statusDisplayText.text = $"{stringOperator()}{Mathf.Round((previousHealthStatus - newValue) * 100)}HP";
        }

        previousHealthStatus = newValue;
        healthSlider.value = newValue;
    }

    public void UpdateManaSlider(float newValue)
    {
        if (newValue == 1 || newValue == previousHealthStatus)
            return;

        //Checks if first time load
        if (previousManaStatus != 0)
        {
            TMP_Text statusDisplayText = Instantiate(statusDisplayObject, displayParent).GetComponent<TMP_Text>();

            string stringOperator()
            {
                if (previousManaStatus > newValue)
                    return "-";
                else
                    return "+";
            }

            statusDisplayText.text = $"{stringOperator()}{Mathf.Round((previousManaStatus - newValue) * 100)}MN";
        }

        previousManaStatus = newValue;
        manaSlider.value = newValue;
    }

    public void AddNewBuffImage(Sprite buffSprite)
    {
        Image newImage = Instantiate(buffImage, buffsParent);
        newImage.sprite = buffSprite;
    }

    public void RemoveBuffImages()
    {
        for (int i = 0; i < buffsParent.childCount; i++)
            Destroy(buffsParent.GetChild(i).gameObject);
    }
}
