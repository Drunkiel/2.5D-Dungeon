using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    private static readonly string mainPath = Application.dataPath + "/Game";
    public static readonly string savePath = mainPath + "/Saves/";
    public static readonly string mapSavePath = mainPath + "/Maps/";
    public SaveData _data;
    public SettingsData _settingsData;

    void Awake()
    {
        if (!Directory.Exists(mainPath))
        {
            Directory.CreateDirectory(mainPath + "/Maps");
            Directory.CreateDirectory(mainPath + "/Saves");

            //Creating items directory
            Directory.CreateDirectory(mainPath + "/Items");
            Directory.CreateDirectory(mainPath + "/Items/Weapons");
            Directory.CreateDirectory(mainPath + "/Items/Weapons/Warrior");
            Directory.CreateDirectory(mainPath + "/Items/Weapons/Archer");
            Directory.CreateDirectory(mainPath + "/Items/Weapons/Mage");

            Directory.CreateDirectory(mainPath + "/Items/Armor");
            Directory.CreateDirectory(mainPath + "/Items/Armor/Warrior");
            Directory.CreateDirectory(mainPath + "/Items/Armor/Archer");
            Directory.CreateDirectory(mainPath + "/Items/Armor/Mage");

            Directory.CreateDirectory(mainPath + "/Items/Collectable");
        }
    }

    public virtual void Save(string path)
    {
        //EXAMPLE
        //Here open file
        FileStream saveFile = new(path, FileMode.OpenOrCreate);

        //Here collect data to save

        //Here save data to file
        string jsonData = JsonUtility.ToJson(_settingsData, true);

        saveFile.Close();
        File.WriteAllText(path, jsonData);
    }

    public virtual void Load(string path)
    {
        //EXAMPLE
        //Here load data from file
        string saveFile = ReadFromFile(path);
        JsonUtility.FromJsonOverwrite(saveFile, _settingsData);

        //Here override game data
    }

    protected string ReadFromFile(string path)
    {
        using StreamReader Reader = new(path);
        string saveFile = Reader.ReadToEnd();
        return saveFile;
    }

    public List<string> GetGameFiles(string path)
    {
        List<string> foundedFiles = new();

        //Gets all files from folder
        DirectoryInfo dir = new(path);
        FileInfo[] mainInfo = dir.GetFiles("*.json");

        //Fetch files through main directory
        foreach (FileInfo singleFile in mainInfo)
            foundedFiles.Add(singleFile.Name);   

        //Fetch files through sub directory's
        DirectoryInfo[] directoryInfos = dir.GetDirectories();
        foreach (DirectoryInfo dirInfo in directoryInfos)
        {
            FileInfo[] fileInfos = dirInfo.GetFiles("*.json");
            foreach (FileInfo fileInfo in fileInfos)
                foundedFiles.Add($"{dirInfo.Name}/{fileInfo.Name}");
        }

        return foundedFiles;
    }

    public static void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] textureBytes = texture.EncodeToPNG();

        // Saving to file
        File.WriteAllBytes(path, textureBytes);
    }
}