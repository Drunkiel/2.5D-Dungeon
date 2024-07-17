using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static readonly string savePath = Application.dataPath + "/ala/Saves/";
    public static readonly string mapSavePath = Application.dataPath + "/ala/Maps/";
    public SaveData _data;
    public MapData _da;
    public SettingsData _settingsData;

    void Awake()
    {
        if (!Directory.Exists(Application.dataPath + "/ala"))
        {
            Directory.CreateDirectory(Application.dataPath + "/ala/Maps");
            Directory.CreateDirectory(Application.dataPath + "/ala/Saves");

            File.Create(Application.dataPath + "/ala/Maps/test.json");
            Save(mapSavePath + "test.json", _da);
        }
    }

    public virtual void Save(string path, Object objectData)
    {
        //EXAMPLE
        //Here open file
        FileStream saveFile = new(path, FileMode.OpenOrCreate);

        //Here collect data to save

        //Here save data to file
        string jsonData = JsonUtility.ToJson(objectData, true);

        saveFile.Close();
        File.WriteAllText(path, jsonData);
    }

    public virtual void Load(string path, Object objectData)
    {
        //EXAMPLE
        //Here load data from file
        string saveFile = ReadFromFile(path);
        JsonUtility.FromJsonOverwrite(saveFile, objectData);

        //Here override game data
    }

    protected string ReadFromFile(string path)
    {
        using StreamReader Reader = new(path);
        string saveFile = Reader.ReadToEnd();
        return saveFile;
    }


    public static void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] textureBytes = texture.EncodeToPNG();

        // Saving to file
        File.WriteAllBytes(path, textureBytes);
    }
}