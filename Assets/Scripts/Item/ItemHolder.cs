using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class ItemHolder : SaveLoadSystem
{
    public List<ItemData> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();
    public WeaponData weaponData;

    void Start()
    {
        // Load(itemsSavePath + "Weapons/Warrior");
        // Load(itemsSavePath + "Weapons/Archer");
        // Load(itemsSavePath + "Weapons/Mage");

        // Load(itemsSavePath + "Armor/Warrior");
        // Load(itemsSavePath + "Armor/Archer");
        // Load(itemsSavePath + "Armor/Mage");

        // Load(itemsSavePath + "Collectable");
        // Save(itemsSavePath + "/Test.json");
        Load(itemsSavePath);
    }

    public override void Save(string path)
    {
        // Collect data to save
        string jsonData = JsonConvert.SerializeObject(weaponData, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None
        });

        // Save data to file
        File.WriteAllText(path, jsonData);
    }

    public override void Load(string path)
    {
        List<string> itemsLocation = GetGameFiles(path);

        for (int i = 0; i < itemsLocation.Count; i++)
        {
            // Here load data from file
            WeaponData _itemData = ScriptableObject.CreateInstance<WeaponData>();
            //_itemData.ID = (short)i;

            string saveFile = ReadFromFile($"{path}/{itemsLocation[i]}");
            
            // Deserialize
            JsonConvert.PopulateObject(saveFile, _itemData);
            weaponData = _itemData;

            // Checks if item is in standard's
            //_allItems.Add(_itemData);
        }
    }
}