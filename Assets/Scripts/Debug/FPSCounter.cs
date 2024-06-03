using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public float timer;
    public float refresh;
    public float avgFramerate;

    string display = "FPS: {0}";
    private TMP_Text displayText;

    private void Start()
    {
        displayText = GetComponent<TMP_Text>();

        InvokeRepeating(nameof(CountFPS), 0f, 0.5f);
    }

    void CountFPS()
    {
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        displayText.text = string.Format(display, avgFramerate.ToString());
    }
}