using System.IO;
using Newtonsoft.Json;
using UnityEngine;

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
        EntityController _playerController = GameController.instance._player;

        //Override data to save
        _saveData.skinPath = _playerController.GetComponent<EntityLookController>().skinPath;
        _saveData.sceneName = PortalController.instance._currScene.sceneName;
        _saveData.position = new(_playerController.transform.position);
        _saveData._inventoryData = new(_playerController._holdingController._itemController._gearHolder);

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
        EntityController _playerController = GameController.instance._player;
        ItemController _playerItems = _playerController._holdingController._itemController;

        //Load data from file
        string saveFile = ReadFromFile(path);

        //Deserialize
        JsonConvert.PopulateObject(saveFile, _saveData);

        //Override game data
        //Load Skin
        EntityLookController _lookController = _playerController.GetComponent<EntityLookController>();
        _lookController.skinPath = _saveData.skinPath;
        _lookController.SpriteLoader();
        //Set position
        _teleportEvent.positions[0] = _saveData.position.ConvertToVector3() + Vector3.up;
        _teleportEvent.TeleportToScene(_saveData.sceneName);
        //Load inventory
        ItemContainer _itemContainer = ItemContainer.instance;
        if (_saveData._inventoryData.weaponRightID != 0)
            InventoryController.instance.AddToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.weaponRightID, ItemType.Weapon), 3);
        if (_saveData._inventoryData.weaponLeftID != 0)
            InventoryController.instance.AddToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.weaponLeftID, ItemType.Weapon), 4);
        if (_saveData._inventoryData.armorHeadID != 0)
            InventoryController.instance.AddToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.armorHeadID, ItemType.Armor), 0);
        if (_saveData._inventoryData.armorChestplateID != 0)
            InventoryController.instance.AddToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.armorChestplateID, ItemType.Armor), 1);
        if (_saveData._inventoryData.armorBootsID != 0)
            InventoryController.instance.AddToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.armorBootsID, ItemType.Armor), 2);
        //_playerItems.SetWeapon(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.weaponRightID, ItemType.Weapon));
        //_playerItems.SetWeapon(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.weaponLeftID, ItemType.Weapon));

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
