using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ItemHolder : SaveLoadSystem
{
    public List<ItemID> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();
    //public List<Object> allobjects = new();
    public List<GameObject> itemPrefabs = new();

    void Start()
    {
        // Save(itemsSavePath);
        
        Load(itemsSavePath);
    }

    // public override void Save(string path)
    // {
    //     for (int i = 0; i < allobjects.Count; i++)
    //     {
    //         // Collect data to save
    //         string jsonData = JsonConvert.SerializeObject(allobjects[i], Formatting.Indented, new JsonSerializerSettings
    //         {
    //             ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    //             TypeNameHandling = TypeNameHandling.Auto,
    //             PreserveReferencesHandling = PreserveReferencesHandling.None
    //         });

    //         // Save data to file
    //         File.WriteAllText($"{path}/{allobjects[i].name}.json", jsonData);
    //     }
    // }

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
                    _weaponItems.Add(CreateWeaponItem(_newWeapon));
                    break;
                
                //Parse to armor
                case ItemType.Armor:
                    ArmorData _newArmor = ScriptableObject.CreateInstance<ArmorData>();
                    JsonConvert.PopulateObject(saveFile, _newArmor);
                    _newItem = _newArmor;
                    _armorItems.Add(CreateArmorItem(_newArmor));
                    break;
            }

            //If empty drop warning
            if (_newItem == null)
            {
                Debug.LogWarning($"File {itemLocation} does not match any known data type.");
                return;
            }
        }

        _allItems.AddRange(_weaponItems);
        _allItems.AddRange(_armorItems);
        _allItems.AddRange(_collectableItems);
    }

    private ItemID CreateWeaponItem(WeaponData _weaponData)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[0], new Vector2(0, -2), Quaternion.identity).GetComponent<ItemID>();
        WeaponItem _weaponItem = _itemID._weaponItem;

        //Assigning data
        _itemID._itemData = _weaponData._itemData;
        _weaponItem.weaponType = _weaponData.weaponType;
        _weaponItem.holdingType = _weaponData.holdingType;
        _weaponItem.resizable = _weaponData.resizable;

        _itemID.name = _weaponData._itemData.displayedName;
        return _itemID;
    }

    private ItemID CreateArmorItem(ArmorData _armorData)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[1], new Vector2(0, -2), Quaternion.identity).GetComponent<ItemID>();
        ArmorItem _armorItem = _itemID._armorItem;

        //Assigning data
        _itemID._itemData = _armorData._itemData;
        _armorItem.armorType = _armorData.armorType;

        _itemID.name = _armorData._itemData.displayedName;
        return _itemID;
    }
}