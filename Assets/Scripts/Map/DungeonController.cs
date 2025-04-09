using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DungeonController : MonoBehaviour
{
    [SerializeField] private EntityController _coreController;
    public int timeLeft;
    [SerializeField] private Slider timeSlider;

    private void Start()
    {
        timeSlider.maxValue = timeLeft;
        timeSlider.value = timeLeft;
        _coreController._statistics._statsController.UpdateManaSlider(0, _coreController._statistics.maxMana, true);
        _coreController._statistics.maxMana = timeLeft;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        if (timeLeft <= 0)
            yield break;

        yield return new WaitForSeconds(1f);
        timeLeft -= 1;
        timeSlider.value = timeLeft;
        _coreController._statistics.TakeMana(-1, true);

        StartCoroutine(UpdateTimer());
    }
}
