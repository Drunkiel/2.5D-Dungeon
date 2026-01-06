using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveController : SaveLoadSystem
{
    public static SaveController instance;

    public SaveData _saveData;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        try
        {
            Load(savePath + "save.json");
        }
        catch (System.Exception)
        {
            Save(savePath + "save.json");
        }
    }

    public override void Save(string path)
    {
        //Override data to save
        _saveData.skinPath = PlayerController.instance.GetComponent<EntityLookController>().skinPath;

        //Save data to file
        string jsonData = JsonConvert.SerializeObject(_saveData, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        });

        File.WriteAllText(path, jsonData);
    }

    public override void Load(string path)
    {
        //Load data from file
        string saveFile = ReadFromFile(path);

        //Deserialize
        JsonConvert.PopulateObject(saveFile, _saveData);

        //Override game data
        PlayerController.instance.GetComponent<EntityLookController>().skinPath = _saveData.skinPath;
    }

    public void ForceSave()
    {
        Save(savePath + "save.json");
    }
}
