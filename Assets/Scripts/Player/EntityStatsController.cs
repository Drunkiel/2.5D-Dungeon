using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityStatsController : MonoBehaviour
{
    public TMP_Text nameText;
    public Slider healthSlider;
    private float previousHealthStatus;
    public Slider manaSlider;
    private float previousManaStatus;
    [SerializeField] private GameObject statusDisplayObject;
    [SerializeField] private Transform buffsParent;
    [SerializeField] private Image buffImage;

    public void SetName(string newName)
    {
        nameText.text = newName;
    }

    public void SetSliderColor(Entity entity)
    {
        Image sliderImage = healthSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();

        switch (entity)
        {
            case Entity.Enemy:
                sliderImage.color = Color.red;
                break;

            case Entity.Friendly:
                sliderImage.color = Color.green;
                break;
        }
    }

    public void UpdateHealthSlider(float newValue, float maxValue, bool ignore = false)
    {
        if (previousHealthStatus == newValue) 
            return;

        if (previousHealthStatus != 0 && !ignore)
        {
            TMP_Text statusDisplayText = Instantiate(statusDisplayObject, transform.position, transform.rotation).transform.GetChild(0).GetComponent<TMP_Text>();

            string stringOperator()
            {
                if (previousHealthStatus > newValue)
                    return "-";
                else
                    return "+";
            }

            statusDisplayText.color = Color.red;
            statusDisplayText.text = $"{stringOperator()}{Mathf.Round(previousHealthStatus - newValue)}HP";
            statusDisplayText.GetComponent<Animator>().Play($"Random_{Random.Range(1, 4)}");
        }

        previousHealthStatus = newValue;
        healthSlider.value = newValue / maxValue;
    }

    public void UpdateManaSlider(float newValue, float maxValue, bool ignore = false)
    {
        if (previousManaStatus == newValue)
            return;

        if (previousManaStatus != 0 && !ignore)
        {
            TMP_Text statusDisplayText = Instantiate(statusDisplayObject, transform.position, transform.rotation).transform.GetChild(0).GetComponent<TMP_Text>();

            string stringOperator()
            {
                if (previousManaStatus > newValue)
                    return "-";
                else
                    return "+";
            }

            statusDisplayText.color = Color.blue;
            statusDisplayText.text = $"{stringOperator()}{Mathf.Round(previousManaStatus - newValue)}MN";
            statusDisplayText.GetComponent<Animator>().Play($"Random_{Random.Range(1, 4)}");
        }

        previousManaStatus = newValue;
        manaSlider.value = newValue / maxValue;
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
