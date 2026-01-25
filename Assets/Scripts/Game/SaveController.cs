using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class EasyVector3
{
    public float x;
    public float y;
    public float z;

    public EasyVector3() { }

    public EasyVector3(float x, float y, float z)
    {
        this.x = Mathf.Round(x * 100f) / 100f;
        this.y = Mathf.Round(y * 100f) / 100f;
        this.z = Mathf.Round(z * 100f) / 100f;
    }

    public EasyVector3(Vector3 value)
    {
        x = Mathf.Round(value.x * 100f) / 100f;
        y = Mathf.Round(value.y * 100f) / 100f;
        z = Mathf.Round(value.z * 100f) / 100f;
    }

    public Vector3 ConvertToVector3()
    {
        return new(x, y, z);
    }
}

public class SaveController : SaveLoadSystem
{
    public static SaveController instance;

    public SaveData _saveData;
    private TeleportEvent _teleportEvent;

    private void Awake()
    {
        instance = this;
        _teleportEvent = GetComponent<TeleportEvent>();

        // try
        // {
        //     Load(savePath + "settings.json");
        // }
        // catch (System.Exception)
        // {
        //     Save(savePath + "settings.json");
        // }
    }

    public override void Save(string path)
    {
        //Override data to save
        _saveData.skinPath = GameController.instance._player.GetComponent<EntityLookController>().skinPath;
        _saveData.sceneName = PortalController.instance._currScene.sceneName;
        _saveData.position = new(GameController.instance._player.transform.position);

        //Save data to file
        string jsonData = JsonConvert.SerializeObject(_saveData, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        });

        File.WriteAllText(path, jsonData);
        print("Saved");
    }

    public override void Load(string path)
    {
        //Load data from file
        string saveFile = ReadFromFile(path);

        //Deserialize
        JsonConvert.PopulateObject(saveFile, _saveData);

        //Override game data
        EntityLookController _lookController = GameController.instance._player.GetComponent<EntityLookController>();
        _lookController.skinPath = _saveData.skinPath;
        _lookController.SpriteLoader();
        _teleportEvent.positions[0] = _saveData.position.ConvertToVector3();
        _teleportEvent.TeleportToScene(_saveData.sceneName);

        print("Loaded");
    }

    public void ForceSave()
    {
        Save(savePath + "save.json");
    }

    public void ForceLoad()
    {
        Load(savePath + "save.json");
    }
}
