using System.IO;

public class SettingsController : SaveLoadSystem
{
    public static SettingsController instance;

    public GraphicsSettings _graphicsSettings;
    //public VolumeController _volumeController;

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
        path = savePath + "settings.json";
        
        //Creating or open file
        FileStream saveFile = new(path, FileMode.OpenOrCreate);

        //Overrite data to save
        //Save graphics data
        // _settingsData.resolutionIndex = resolutionDropdown.value;
        // _settingsData.fullscreenOn = fullscreenToggle.isOn;
        // _settingsData.maxFPS = int.Parse(fpsInput.text);
        // _settingsData.vSync = vSyncToggle.isOn;
        // _settingsData.qualityIndex = qualityDropdown.value;

        // //Save volume data
        // _settingsData.backgroundVolume = _volumeController.backgroundVolumeSlider.value;
        // _settingsData.effectsVolume = _volumeController.effectsVolumeSlider.value;

        // //Saving data
        // string jsonData = JsonUtility.ToJson(_settingsData, true);

        saveFile.Close();
        //File.WriteAllText(path, jsonData);
    }

    public override void Load(string path)
    {
        path = savePath + "settings.json";

        //Load data from file
        string saveFile = ReadFromFile(path);
        // JsonUtility.FromJsonOverwrite(saveFile, _settingsData);

        // //Load graphics data
        // resolutionDropdown.value = _settingsData.resolutionIndex;
        // fullscreenToggle.isOn = _settingsData.fullscreenOn;
        // fpsInput.text = _settingsData.maxFPS.ToString();
        // vSyncToggle.isOn = _settingsData.vSync;
        // qualityDropdown.value = _settingsData.qualityIndex;

        // //Load volume data
        // _volumeController.backgroundVolumeSlider.value = _settingsData.backgroundVolume;
        // _volumeController.UpdateBackgroundVolume();
        // _volumeController.effectsVolumeSlider.value = _settingsData.effectsVolume;
    }
}