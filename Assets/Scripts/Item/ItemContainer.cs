using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class ItemContainer : SaveLoadSystem
{
    public static ItemContainer instance;

    public List<ItemID> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();
    public List<GameObject> itemPrefabs = new();

    void Awake()
    {
        instance = this;
        LoadStuff();
    }

    public void LoadStuff()
    {
        Load(itemsSavePath + "Weapons/Warrior");
        Load(itemsSavePath + "Weapons/Archer");
        Load(itemsSavePath + "Weapons/Mage");

        Load(itemsSavePath + "Armor/Warrior");
        Load(itemsSavePath + "Armor/Archer");
        Load(itemsSavePath + "Armor/Mage");

        Load(itemsSavePath + "Collectable");
        
        _allItems.AddRange(_weaponItems);
        _allItems.AddRange(_armorItems);
        _allItems.AddRange(_collectableItems);
    }

    public void UnLoadStuff()
    {
        foreach(ItemID _itemID in _allItems)
            Destroy(_itemID.gameObject);

        _allItems.Clear();
        _weaponItems.Clear();
        _armorItems.Clear();
        _collectableItems.Clear();
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
                    _weaponItems.Add(CreateWeaponItem(_newWeapon, path));
                    break;
                
                //Parse to armor
                case ItemType.Armor:
                    ArmorData _newArmor = ScriptableObject.CreateInstance<ArmorData>();
                    JsonConvert.PopulateObject(saveFile, _newArmor);
                    _newItem = _newArmor;
                    _armorItems.Add(CreateArmorItem(_newArmor, path));
                    break;

                case ItemType.Collectable:
                    CollectableData _newCollectable = ScriptableObject.CreateInstance<CollectableData>();
                    JsonConvert.PopulateObject(saveFile, _newCollectable);
                    _newItem = _newCollectable;
                    _collectableItems.Add(CreateCollectableItem(_newCollectable, path));
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

    public ItemID GetItemByName(string itemName)
    {
        for (int i = 0; i < _allItems.Count; i++)
        {
            if(_allItems[i]._itemData.displayedName == itemName)
                return _allItems[i];
        }

        return null;
    }

    public ItemID GetItemByNameAndType(string itemName, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Armor:
                for (int i = 0; i < _armorItems.Count; i++)
                {
                    if(_armorItems[i]._itemData.displayedName == itemName)
                        return _armorItems[i];
                }
                return null;

            case ItemType.Weapon:
                for (int i = 0; i < _weaponItems.Count; i++)
                {
                    if(_weaponItems[i]._itemData.displayedName == itemName)
                        return _weaponItems[i];
                }
                return null;

            case ItemType.Collectable:
                for (int i = 0; i < _collectableItems.Count; i++)
                {
                    if(_collectableItems[i]._itemData.displayedName == itemName)
                        return _collectableItems[i];
                }
                return null;
        }

        return null;
    }

    private ItemID CreateWeaponItem(WeaponData _weaponData, string path)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[0], new Vector2(0, -2), Quaternion.identity, transform).GetComponent<ItemID>();
        WeaponItem _weaponItem = _itemID._weaponItem;

        //Load values
        _itemID._itemData = _weaponData._itemData;
        _weaponItem.weaponType = _weaponData.weaponType;
        _weaponItem.holdingType = _weaponData.holdingType;
        _weaponItem.transform.localScale = new(_weaponData.size.x, _weaponData.size.y, 1);
        _itemID.name = _weaponData._itemData.displayedName;

        //Create a sprite
        int orderInLayer = 0;
        switch(_weaponItem.holdingType)
        {
            case WeaponHoldingType.Right_Hand:
                orderInLayer = 5;
                break;

            case WeaponHoldingType.Left_Hand:
                orderInLayer = 2;
                break;

            case WeaponHoldingType.Both_Hands:
                orderInLayer = 8;
                break;
        }

        LoadTexture(_weaponItem, $"{path}/{_weaponData._itemData.spritePath}", orderInLayer, 20f);
        _weaponItem.iconSprite = _itemID._itemData.GetSprite($"{path}/{_weaponData._itemData.spriteIconPath}", 20f);

        return _itemID;
    }

    private ItemID CreateArmorItem(ArmorData _armorData, string path)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[1], new Vector2(0, -2), Quaternion.identity, transform).GetComponent<ItemID>();
        ArmorItem _armorItem = _itemID._armorItem;

        //Load values
        _itemID._itemData = _armorData._itemData;
        _armorItem.armorType = _armorData.armorType;
        _itemID.name = _armorData._itemData.displayedName;

        //Create a sprite
        int orderInLayer = 5;
        if (_armorItem.armorType == ArmorType.Chestplate)
            orderInLayer = 3;

        LoadTexture(_armorItem, $"{path}/{_armorData._itemData.spritePath}", orderInLayer, 100f);
        _armorItem.iconSprite = _itemID._itemData.GetSprite($"{path}/{_armorData._itemData.spriteIconPath}", 20f);

        return _itemID;
    }

    private ItemID CreateCollectableItem(CollectableData _collectableData, string path)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[2], new Vector2(0, -2), Quaternion.identity, transform).GetComponent<ItemID>();
        CollectableItem _collectableItem = _itemID._collectableItem;

        //Load values
        _itemID._itemData = _collectableData._itemData;
        _itemID.name = _collectableData._itemData.displayedName;

        //Create a sprite
        LoadTexture(_collectableItem, $"{path}/{_collectableData._itemData.spritePath}", 5, 20f);
        _collectableItem.iconSprite = _itemID._itemData.GetSprite($"{path}/{_collectableData._itemData.spriteIconPath}", 20f);

        return _itemID;
    }

    private void LoadTexture(Object itemObject, string path, int orderInLayer, float pixelsPerUnit)
    {
        SpriteRenderer spriteRenderer = itemObject.GetComponent<Transform>().GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemObject.GetComponent<ItemID>()._itemData.GetSprite(path, pixelsPerUnit);
        spriteRenderer.sortingOrder = orderInLayer;
    }
}