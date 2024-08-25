using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    private static readonly string mainPath = Application.dataPath + "/Game";
    public static readonly string savePath = mainPath + "/Saves/";
    public static readonly string mapSavePath = mainPath + "/Maps/";
    public static readonly string itemsSavePath = mainPath + "/Items/";
    public static readonly string skillsSavePath = mainPath + "/Skills/";
    public static readonly string skinsSavePath = mainPath + "/Skins/";

    void Awake()
    {
        if (!Directory.Exists(mainPath))
        {
            Directory.CreateDirectory(mainPath + "/Maps");
            Directory.CreateDirectory(mainPath + "/Saves");
            Directory.CreateDirectory(mainPath + "/Skills");

            //Creating items directory
            Directory.CreateDirectory(itemsSavePath);
            Directory.CreateDirectory(itemsSavePath + "Weapons");
            Directory.CreateDirectory(itemsSavePath + "Weapons/Warrior");
            Directory.CreateDirectory(itemsSavePath + "Weapons/Archer");
            Directory.CreateDirectory(itemsSavePath + "Weapons/Mage");

            Directory.CreateDirectory(itemsSavePath + "Armor");
            Directory.CreateDirectory(itemsSavePath + "Armor/Warrior");
            Directory.CreateDirectory(itemsSavePath + "Armor/Archer");
            Directory.CreateDirectory(itemsSavePath + "Armor/Mage");

            Directory.CreateDirectory(itemsSavePath + "Collectable");
        }
    }

    public virtual void Save(string path)
    {
        //EXAMPLE
        //Here save data to file
        string jsonData = JsonConvert.SerializeObject(new(), Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        });

        File.WriteAllText(path, jsonData);
    }

    public virtual void Load(string path)
    {
        //EXAMPLE
        //Here load data from file
        string saveFile = ReadFromFile($"{path}/");
            
        // Deserialize
        JsonConvert.PopulateObject(saveFile, new());

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

    public Sprite GetSprite(string path, float pixelsPerUnit)
    {
        //Create new texture
        byte[] spriteData = File.ReadAllBytes(path);
        Texture2D texture = new(2, 2)
        {
            filterMode = FilterMode.Point
        };

        //Assigning data
        if (texture.LoadImage(spriteData))
        {
            //Convert texture to sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            return sprite;
        }

        return null;
    }

    public static void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] textureBytes = texture.EncodeToPNG();

        // Saving to file
        File.WriteAllBytes(path, textureBytes);
    }
}