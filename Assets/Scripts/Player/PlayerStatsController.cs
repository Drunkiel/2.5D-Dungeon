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

    public void UpdateHealthSlider(float newValue)
    {
        healthSlider.gameObject.SetActive(true);
        healthSlider.value = newValue / PlayerController.instance._statistics.maxHealth;

        StartCoroutine(HideSlider(healthSlider.gameObject));
    }

    IEnumerator HideSlider(GameObject slider)
    {
        yield return new WaitForSeconds(2);

        slider.SetActive(false);
    }
}
