using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class GraphicsSettings : MonoBehaviour
{
    public bool isFullscreen;
    public int resolutionIndex;
    public int qualityIndex;
    public int fpsLimit;
    public bool isVSync;
    
    [SerializeField] private TMP_Text fullscreenText;
    [SerializeField] private TMP_Text resolutionText;
    [SerializeField] private TMP_Text qualityText;
    [SerializeField] private TMP_Text vSyncText;
    [SerializeField] private TMP_InputField fpsInput;

    private readonly List<Vector2Int> screenResolutions = new() { new(3840, 2160), new(2560, 1440), new(1920, 1080), new(1280, 720) };
    private readonly List<string> quality = new() { "Low", "Medium", "High" };

    public OpenCloseUI _openCloseUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _openCloseUI.OpenClose();
    }

    public void UpdateQuality(bool skip = false)
    {
        if (!skip)
        {
            qualityIndex++;
            if (qualityIndex >= quality.Count)
                qualityIndex = 0;
        }

        qualityText.text = quality[qualityIndex];
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void UpdateVSync(bool skip = false)
    {
        if (!skip)
            isVSync = !isVSync;

        if (isVSync)
            QualitySettings.vSyncCount = 1;
        else
        {
            QualitySettings.vSyncCount = 0;
            UpdateFPS();
        }

        vSyncText.text = isVSync ? "On" : "Off";
    }

    public void UpdateFullscreen(bool skip = false)
    {
        if (!skip)
            isFullscreen = !isFullscreen;

        UpdateResolution(true);
        fullscreenText.text = isFullscreen ? "On" : "Off";
    }

    public void UpdateResolution(bool skip = false)
    {
        if (!skip)
        {
            resolutionIndex++;
            if (resolutionIndex >= screenResolutions.Count)
                resolutionIndex = 0;
        }

        int width = screenResolutions[resolutionIndex].x;
        int height = screenResolutions[resolutionIndex].y;

        resolutionText.text = $"{width}x{height}";
        Screen.SetResolution(
            width,
            height,
            isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed
        );
    }

    public void UpdateFPS()
    {
        int newMaxFPS;

        try
        {
            newMaxFPS = int.Parse(fpsInput.text);
        }
        catch (Exception)
        {
            newMaxFPS = 0;
        }

        if (newMaxFPS < 30)
        {
            fpsInput.text = "30";
            newMaxFPS = 30;
        }

        if (QualitySettings.vSyncCount != 0)
            return;

        Application.targetFrameRate = newMaxFPS;
    }
}
