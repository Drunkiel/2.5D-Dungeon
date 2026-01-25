using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class ItemContainer : SaveLoadSystem
{
    public static ItemContainer instance;

    public List<WeaponData> _weaponItems = new();
    public List<ArmorData> _armorItems = new();
    public List<CollectableData> _collectableItems = new();
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
    }

    public void UnLoadStuff()
    {
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

            //Test if founded json is parsable to other types
            ItemDataParser _itemParser = ScriptableObject.CreateInstance<ItemDataParser>();
            JsonConvert.PopulateObject(saveFile, _itemParser);

            switch (_itemParser._itemData.itemType)
            {
                //Parse to weapon
                case ItemType.Weapon:
                    WeaponData _newWeapon = ScriptableObject.CreateInstance<WeaponData>();
                    JsonConvert.PopulateObject(saveFile, _newWeapon);
                    InitializeWeapon(_newWeapon, path);
                    _weaponItems.Add(_newWeapon);
                    break;

                //Parse to armor
                case ItemType.Armor:
                    ArmorData _newArmor = ScriptableObject.CreateInstance<ArmorData>();
                    JsonConvert.PopulateObject(saveFile, _newArmor);
                    InitializeArmor(_newArmor, path);
                    _armorItems.Add(_newArmor);
                    break;

                case ItemType.Collectable:
                    CollectableData _newCollectable = ScriptableObject.CreateInstance<CollectableData>();
                    JsonConvert.PopulateObject(saveFile, _newCollectable);
                    InitializeCollectable(_newCollectable, path);
                    _collectableItems.Add(_newCollectable);
                    break;
            }
        }
    }

    private void InitializeWeapon(WeaponData _weaponData, string path)
    {
        if (!string.IsNullOrEmpty(_weaponData._itemData.spritePathFront))
            _weaponData.itemSpriteFront = GetSprite($"{path}/{_weaponData._itemData.spritePathFront}", 20f);

        if (!string.IsNullOrEmpty(_weaponData._itemData.spritePathBack))
            _weaponData.itemSpriteBack = GetSprite($"{path}/{_weaponData._itemData.spritePathBack}", 20f);

        _weaponData.iconSprite = GetSprite($"{path}/{_weaponData._itemData.spriteIconPath}", 20f);
    }

    private void InitializeArmor(ArmorData _armorData, string path)
    {
        if (!string.IsNullOrEmpty(_armorData._itemData.spritePathFront))
            _armorData.itemSpriteFront = GetSprite($"{path}/{_armorData._itemData.spritePathFront}", 100f);

        if (!string.IsNullOrEmpty(_armorData._itemData.spritePathBack))
            _armorData.itemSpriteBack = GetSprite($"{path}/{_armorData._itemData.spritePathBack}", 100f);

        _armorData.iconSprite = GetSprite($"{path}/{_armorData._itemData.spriteIconPath}", 20f);
    }

    private void InitializeCollectable(CollectableData _collectableData, string path)
    {
        if (!string.IsNullOrEmpty(_collectableData._itemData.spritePathFront))
            _collectableData.itemSprite = GetSprite($"{path}/{_collectableData._itemData.spritePathFront}", 20f);

        _collectableData.iconSprite = GetSprite($"{path}/{_collectableData._itemData.spriteIconPath}", 20f);
    }

    public ItemID GetItemByName(string itemName)
    {
        // for (int i = 0; i < _allItems.Count; i++)
        // {
        //     if (_allItems[i]._itemData.displayedName == itemName)
        //         return _allItems[i];
        // }

        return null;
    }

    public ItemID GetItemByNameAndType(string itemName, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                for (int i = 0; i < _weaponItems.Count; i++)
                {
                    if (_weaponItems[i]._itemData.displayedName == itemName)
                        return CreateWeaponItem(_weaponItems[i]);
                }
                return null;

            case ItemType.Armor:
                for (int i = 0; i < _armorItems.Count; i++)
                {
                    if (_armorItems[i]._itemData.displayedName == itemName)
                        return CreateArmorItem(_armorItems[i]);
                }
                return null;

            case ItemType.Collectable:
                for (int i = 0; i < _collectableItems.Count; i++)
                {
                    if (_collectableItems[i]._itemData.displayedName == itemName)
                        return CreateCollectableItem(_collectableItems[i]);
                }
                return null;
        }

        return null;
    }

    private ItemID CreateWeaponItem(WeaponData _weaponData)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[0], new Vector2(0, -10), Quaternion.identity, transform).GetComponent<ItemID>();
        WeaponItem _weaponItem = _itemID._weaponItem;

        //Load values
        _itemID._itemData = _weaponData._itemData;
        _weaponItem.weaponType = _weaponData.weaponType;
        _weaponItem.holdingType = _weaponData.holdingType;
        _weaponItem.resizable = _weaponData.resizable;
        _weaponItem.transform.localScale = new(_weaponData.size.x, _weaponData.size.y, 1);
        _itemID.name = _weaponData._itemData.displayedName;

        //Create a sprite
        int orderInLayer = 0;
        switch (_weaponItem.holdingType)
        {
            case WeaponHoldingType.Right_Hand:
                orderInLayer = 6;
                break;

            case WeaponHoldingType.Left_Hand:
                orderInLayer = 2;
                break;

            case WeaponHoldingType.Both_Hands:
                orderInLayer = 8;
                break;
        }

        if (!string.IsNullOrEmpty(_weaponData._itemData.spritePathFront))
            LoadTexture(_weaponItem, _weaponData.itemSpriteFront, orderInLayer);

        if (!string.IsNullOrEmpty(_weaponData._itemData.spritePathBack))
            LoadTexture(_weaponItem, _weaponData.itemSpriteBack, orderInLayer, false);

        _weaponItem.iconSprite = _weaponData.iconSprite;

        return _itemID;
    }

    private ItemID CreateArmorItem(ArmorData _armorData)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[1], new Vector2(1, -10), Quaternion.identity, transform).GetComponent<ItemID>();
        ArmorItem _armorItem = _itemID._armorItem;

        //Load values
        _itemID._itemData = _armorData._itemData;
        _armorItem.armorType = _armorData.armorType;
        _itemID.name = _armorData._itemData.displayedName;

        //Create a sprite
        int orderInLayer = 5;
        if (_armorItem.armorType == ArmorType.Chestplate)
            orderInLayer = 3;

        if (!string.IsNullOrEmpty(_armorData._itemData.spritePathFront))
            _armorItem.itemSpriteFront = LoadTexture(_armorItem, _armorData.itemSpriteFront, orderInLayer);

        if (!string.IsNullOrEmpty(_armorData._itemData.spritePathBack))
            _armorItem.itemSpriteBack = LoadTexture(_armorItem, _armorData.itemSpriteBack, orderInLayer, false);

        _armorItem.iconSprite = _armorData.iconSprite;

        return _itemID;
    }

    private ItemID CreateCollectableItem(CollectableData _collectableData)
    {
        //Creating new item
        ItemID _itemID = Instantiate(itemPrefabs[2], new Vector2(-1, -10), Quaternion.identity, transform).GetComponent<ItemID>();
        CollectableItem _collectableItem = _itemID._collectableItem;

        //Load values
        _itemID._itemData = _collectableData._itemData;
        _itemID.name = _collectableData._itemData.displayedName;

        //Create a sprite
        if (!string.IsNullOrEmpty(_collectableData._itemData.spritePathFront))
            LoadTexture(_collectableItem, _collectableData.itemSprite, 5);
        _collectableItem.iconSprite = _collectableData.iconSprite;

        return _itemID;
    }

    private Sprite LoadTexture(Object itemObject, Sprite sprite, int orderInLayer, bool assign = true)
    {
        if (assign)
        {
            SpriteRenderer spriteRenderer = itemObject.GetComponent<Transform>().GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = orderInLayer;
        }

        return sprite;
    }
}