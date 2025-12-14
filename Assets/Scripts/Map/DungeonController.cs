using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Clock
{
    public int hours;
    public int minutes;
    public int seconds;

    public int timeLeft;

    public void Initialize()
    {
        timeLeft = GetTimeInSeconds();
    }

    public int GetTimeInSeconds()
    {
        return hours * 3600 + minutes * 60 + seconds;
    }

    public void UpdateClock(int deltaTime)
    {
        timeLeft -= deltaTime;

        if (timeLeft < 0)
            timeLeft = 0;

        hours = timeLeft / 3600;
        minutes = timeLeft % 3600 / 60;
        seconds = timeLeft % 60;
    }

    public string DisplayTime()
    {
        if (hours > 0)
            return $"{hours}h{minutes:00}m{seconds:00}s";
        else if (minutes > 0)
            return $"{minutes}m{seconds:00}s";
        else
            return $"{seconds}s";
    }
}


public class DungeonController : MonoBehaviour
{
    public static DungeonController instance;
    [SerializeField] private EntityController _coreController;
    public Clock _clock;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text timeText;

    private void Start()
    {
        instance = this;
        _clock.timeLeft = _clock.GetTimeInSeconds();
        timeSlider.maxValue = _clock.timeLeft;
        timeSlider.value = timeSlider.maxValue;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        if (_clock.timeLeft <= 0)
            yield break;

        yield return new WaitForSeconds(1f);
        _clock.UpdateClock(1);
        timeSlider.value = _clock.timeLeft;
        timeText.text = _clock.DisplayTime();

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
