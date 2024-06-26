using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    public static PlayerStatsController instance;

    public Slider healthSlider;
    public Slider manaSlider;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateHealthSlider(PlayerController.instance._statistics.maxHealth);
        UpdateManaSlider(PlayerController.instance._statistics.maxMana);
    }

    public void UpdateHealthSlider(float newValue)
    {
        healthSlider.gameObject.SetActive(true);

        StartCoroutine(HideSlider(healthSlider, newValue / PlayerController.instance._statistics.maxHealth));
    }

    public void UpdateManaSlider(float newValue)
    {
        manaSlider.gameObject.SetActive(true);

        StartCoroutine(HideSlider(manaSlider, newValue / PlayerController.instance._statistics.maxMana));
    }


    IEnumerator HideSlider(Slider slider, float newValue)
    {
        float startValue = slider.value;
        float time = 0f;

        while (time < 0.5f)
        {
            time += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, newValue, time / 0.5f);
            yield return null;
        }

        yield return new WaitForSeconds(10);

        slider.gameObject.SetActive(false);
    }
}
