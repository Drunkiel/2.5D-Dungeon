using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class ItemHolder : SaveLoadSystem
{
    public List<ItemData> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();
    public List<Object> allobjects = new();
    public List<Object> objects = new();

    void Start()
    {
        Save(itemsSavePath);
        
        Load(itemsSavePath);
    }

    public override void Save(string path)
    {
        for (int i = 0; i < allobjects.Count; i++)
        {
            // Collect data to save
            string jsonData = JsonConvert.SerializeObject(allobjects[i], Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                PreserveReferencesHandling = PreserveReferencesHandling.None
            });

            // Save data to file
            File.WriteAllText($"{path}/{allobjects[i].name}.json", jsonData);
        }
    }

    public override void Load(string path)
    {
        List<string> itemsLocation = GetGameFiles(path);

        foreach (var itemLocation in itemsLocation)
        {
            string saveFile = ReadFromFile($"{path}/{itemLocation}");
            Object _newItem = new();

            //Test if founded json is parsable to other types
            ItemDataParser _itemParser = ScriptableObject.CreateInstance<ItemDataParser>();
            JsonConvert.PopulateObject(saveFile, _itemParser);

            switch(_itemParser._itemData.itemType)
            {
                //Parse to weapon
                case ItemType.Weapon:
                    WeaponData _newWeapon = ScriptableObject.CreateInstance<WeaponData>();
                    JsonConvert.PopulateObject(saveFile, _newWeapon);
                    _newItem = _newWeapon;
                    break;
                
                case ItemType.Armor:
                    //Parse to armor
                    ArmorData _newArmor = ScriptableObject.CreateInstance<ArmorData>();
                    JsonConvert.PopulateObject(saveFile, _newArmor);
                    _newItem = _newArmor;
                    break;
            }

            //If empty drop warning
            if (_newItem == null)
                Debug.LogWarning($"File {itemLocation} does not match any known data type.");

            objects.Add(_newItem);
        }
    }
}