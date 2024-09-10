using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    public Slider healthSlider;
    [SerializeField] private float previousHealthStatus;
    public Slider manaSlider;
    [SerializeField] private float previousManaStatus;
    [SerializeField] private Transform displayParent;
    [SerializeField] private GameObject statusDisplayObject;

    public void UpdateHealthSlider(float newValue, bool hide = false, bool firstLoad = false)
    {
        if (newValue == 1 || newValue == previousHealthStatus)
            return;

        if (!firstLoad)
        {
            TMP_Text statusDisplayText = Instantiate(statusDisplayObject, displayParent).GetComponent<TMP_Text>();
            healthSlider.gameObject.SetActive(true);

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
        StartCoroutine(HideSlider(healthSlider, newValue, hide));
    }

    public void UpdateManaSlider(float newValue, bool hide = false, bool firstLoad = false)
    {
        if (newValue == 1 || newValue == previousHealthStatus)
            return;

        if (!firstLoad)
        {
            TMP_Text statusDisplayText = Instantiate(statusDisplayObject, displayParent).GetComponent<TMP_Text>();
            manaSlider.gameObject.SetActive(true);

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
        StartCoroutine(HideSlider(manaSlider, newValue, hide));
    }


    IEnumerator HideSlider(Slider slider, float newValue, bool hide = false)
    {
        float startValue = slider.value;
        float time = 0f;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, newValue, time / 0.5f);
            yield return null;
        }

        if (hide)
            yield break;

        yield return new WaitForSeconds(10);

        slider.gameObject.SetActive(false);
    }
}
