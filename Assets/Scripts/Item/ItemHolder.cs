using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

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
        // Load(itemsSavePath + "Weapons/Warrior");
        // Load(itemsSavePath + "Weapons/Archer");
        // Load(itemsSavePath + "Weapons/Mage");

        Load(itemsSavePath + "Armor/Warrior");
        Load(itemsSavePath + "Armor/Archer");
        Load(itemsSavePath + "Armor/Mage");

        _allItems.AddRange(_weaponItems);
        _allItems.AddRange(_armorItems);
        _allItems.AddRange(_collectableItems);
        //Load(itemsSavePath + "Collectable");
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
                    _weaponItems.Add(CreateWeaponItem(_newWeapon, path));
                    break;
                
                //Parse to armor
                case ItemType.Armor:
                    ArmorData _newArmor = ScriptableObject.CreateInstance<ArmorData>();
                    JsonConvert.PopulateObject(saveFile, _newArmor);
                    _newItem = _newArmor;
                    _armorItems.Add(CreateArmorItem(_newArmor, path));
                    break;
            }

            //If empty drop warning
            if (_newItem == null)
            {
                Debug.LogWarning($"File {itemLocation} does not match any known data type.");
                return;
            }
        }
    }

    private ItemID CreateWeaponItem(WeaponData _weaponData, string path)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[0], new Vector2(0, -2), Quaternion.identity).GetComponent<ItemID>();
        WeaponItem _weaponItem = _itemID._weaponItem;

        //Create new texture
        byte[] spriteData = File.ReadAllBytes($"{path}/{_weaponData._itemData.spritePath}");
        Texture2D texture = new(2, 2)
        {
            filterMode = FilterMode.Point
        };

        //Assigning data
        if (texture.LoadImage(spriteData))
        {
            //Convert texture to sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            _weaponItem.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        }

        _itemID._itemData = _weaponData._itemData;
        _weaponItem.weaponType = _weaponData.weaponType;
        _weaponItem.holdingType = _weaponData.holdingType;
        _weaponItem.resizable = _weaponData.resizable;
        _itemID.name = _weaponData._itemData.displayedName;
        return _itemID;
    }

    private ItemID CreateArmorItem(ArmorData _armorData, string path)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[1], new Vector2(0, -2), Quaternion.identity).GetComponent<ItemID>();
        ArmorItem _armorItem = _itemID._armorItem;

        //Create new texture
        byte[] spriteData = File.ReadAllBytes($"{path}/{_armorData._itemData.spritePath}");
        Texture2D texture = new(2, 2)
        {
            filterMode = FilterMode.Point
        };

        //Assigning data
        if (texture.LoadImage(spriteData))
        {
            //Convert texture to sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            _armorItem.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = sprite;
        }

        _itemID._itemData = _armorData._itemData;
        _armorItem.armorType = _armorData.armorType;
        _itemID.name = _armorData._itemData.displayedName;
        return _itemID;
    }
}