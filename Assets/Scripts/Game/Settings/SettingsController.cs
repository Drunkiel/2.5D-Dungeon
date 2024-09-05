using System.IO;
using Newtonsoft.Json;

public class SettingsController : SaveLoadSystem
{
    public static SettingsController instance;

    public SettingsData _settingsData;
    public GraphicsSettings _graphicsSettings;
    //public VolumeController _volumeController;

    public OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;

        try
        {
            Load(savePath + "settings.json");
        }
        catch (System.Exception)
        {
            Save(savePath + "settings.json");
        }
    }

    public override void Save(string path)
    {   
        _settingsData.isFullscreen = _graphicsSettings.isFullscreen;
        _settingsData.resolutionIndex = _graphicsSettings.resolutionIndex;
        _settingsData.qualityIndex = _graphicsSettings.qualityIndex;
        _settingsData.fpsLimit = _graphicsSettings.fpsLimit;
        _settingsData.isVSync = _graphicsSettings.isVSync;

        //Here save data to file
        string jsonData = JsonConvert.SerializeObject(_settingsData, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        });

        File.WriteAllText(path, jsonData);
    }

    public override void Load(string path)
    {
        //Here load data from file
        string saveFile = ReadFromFile(path);
            
        // Deserialize
        JsonConvert.PopulateObject(saveFile, _settingsData);

        //Here override game data
        _graphicsSettings.isFullscreen = _settingsData.isFullscreen;
        _graphicsSettings.resolutionIndex = _settingsData.resolutionIndex;
        _graphicsSettings.qualityIndex = _settingsData.qualityIndex;
        _graphicsSettings.fpsLimit = _settingsData.fpsLimit;
        _graphicsSettings.isVSync = _settingsData.isVSync;

        _graphicsSettings.UpdateFullscreen(true);
        _graphicsSettings.UpdateResolution(true);
        _graphicsSettings.UpdateQuality(true);
        _graphicsSettings.UpdateVSync(true);
    }

    public void ManagePauseUI()
    {
        _openCloseUI.OpenClose();
        GameController.isPaused = _openCloseUI.isOpen;
    }
}