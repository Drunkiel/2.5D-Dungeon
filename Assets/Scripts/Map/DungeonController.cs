using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DungeonController : MonoBehaviour
{
    public static DungeonController instance;
    [SerializeField] private EntityController _coreController;
    public int timeLeft;
    [SerializeField] private Slider timeSlider;

    private void Start()
    {
        instance = this;
        timeSlider.maxValue = timeLeft;
        timeSlider.value = timeLeft;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        if (timeLeft <= 0)
            yield break;

        yield return new WaitForSeconds(1f);
        timeLeft -= 1;
        timeSlider.value = timeLeft;

        if (_coreController != null)
            _coreController._statistics.TakeMana(-1, true);

        StartCoroutine(UpdateTimer());
    }

    public void SetCore(EntityController _core)
    {
        _coreController = _core;
        _coreController._statistics.maxMana = (int)timeSlider.maxValue;
        _coreController._statistics._statsController.UpdateManaSlider(0, _coreController._statistics.maxMana, true);
    }
}
