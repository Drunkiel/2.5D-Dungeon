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
    }

    public override void Save(string path)
    {
        EntityController _playerController = GameController.instance._player;

        //Override data to save
        _saveData.skinPath = _playerController.GetComponent<EntityLookController>().skinPath;
        _saveData.sceneName = PortalController.instance._currScene.sceneName;
        _saveData.position = new(_playerController.transform.position);
        _saveData._inventoryData = new(_playerController._holdingController._itemController._gearHolder, InventoryController.instance._inventorySlots);
        _saveData._skillData = new(InventoryController.instance._skillSlots);

        //Save data to file
        string jsonData = JsonConvert.SerializeObject(_saveData, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        });

        try
        {
            File.WriteAllText(path, jsonData);
        }
        catch (System.Exception)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, "Failed to save game", OutputType.Normal);
            return;
        }
        ConsoleController.instance.ChatMessage(SenderType.System, "Game is saved", OutputType.Normal);
    }

    public override void Load(string path)
    {
        EntityController _playerController = GameController.instance._player;

        //Load data from file
        string saveFile = ReadFromFile(path);

        //Deserialize
        JsonConvert.PopulateObject(saveFile, _saveData);

        //Override game data
        //Set position
        LoadPosition();
        //Load inventory
        LoadInventory();
        //Load skills
        LoadSkills();
        //Load Skin
        LoadSkin(_playerController);

        PopUpController.instance.CreatePopUp(PopUpInfo.VisitPlace, "Save is loaded");
    }

    private void LoadPosition()
    {
        _teleportEvent.positions[0] = _saveData.position.ConvertToVector3() + Vector3.up;
        _teleportEvent.TeleportToScene(_saveData.sceneName);
    }

    private void LoadInventory()
    {
        InventoryController _inventoryController = InventoryController.instance;
        ItemContainer _itemContainer = ItemContainer.instance;

        if (_saveData._inventoryData.weaponRightID != 0)
            _inventoryController.LoadToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.weaponRightID, ItemType.Weapon), 3);

        if (_saveData._inventoryData.weaponLeftID != 0)
            _inventoryController.LoadToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.weaponLeftID, ItemType.Weapon), 4);

        if (_saveData._inventoryData.armorHeadID != 0)
            _inventoryController.LoadToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.armorHeadID, ItemType.Armor), 0);

        if (_saveData._inventoryData.armorChestplateID != 0)
            _inventoryController.LoadToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.armorChestplateID, ItemType.Armor), 1);

        if (_saveData._inventoryData.armorBootsID != 0)
            _inventoryController.LoadToGearInventory(_itemContainer.GetItemByIDAndType(_saveData._inventoryData.armorBootsID, ItemType.Armor), 2);

        for (int i = 0; i < _saveData._inventoryData.inventoryItemIDs.Count; i++)
            if (_saveData._inventoryData.inventoryItemIDs[i] != 0)
                _inventoryController.LoadToInventory(_itemContainer.GetItemByID(_saveData._inventoryData.inventoryItemIDs[i]), i);
    }

    private void LoadSkills()
    {
        InventoryController _inventoryController = InventoryController.instance;

        for (int i = 0; i < _inventoryController._skillSlots.Count; i++)
        {
            if (_saveData._skillData.skillIDs[i] != 0)
                _inventoryController.LoadToSkillInventory(SkillContainer.instance.GetSkillByID(_saveData._skillData.skillIDs[i]), i);
        }
    }

    private void LoadSkin(EntityController _playerController)
    {
        EntityLookController _lookController = _playerController.GetComponent<EntityLookController>();
        _lookController.skinPath = _saveData.skinPath;
        _lookController.SpriteLoader();
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
