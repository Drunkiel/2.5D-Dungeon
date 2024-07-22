using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : SaveLoadSystem
{
    public List<ItemData> _allItems = new();
    public List<ItemID> _weaponItems = new();
    public List<ItemID> _armorItems = new();
    public List<ItemID> _collectableItems = new();

    void Start()
    {
        Load(itemsSavePath + "Weapons/Warrior");
        Load(itemsSavePath + "Weapons/Archer");
        Load(itemsSavePath + "Weapons/Mage");

        Load(itemsSavePath + "Armor/Warrior");
        Load(itemsSavePath + "Armor/Archer");
        Load(itemsSavePath + "Armor/Mage");

        Load(itemsSavePath + "Collectable");
    }

    public override void Load(string path)
    {
        List<string> itemsLocation = GetGameFiles(path);

        for (int i = 0; i < itemsLocation.Count; i++)
        {
            //Here load data from file
            ItemData _itemData = ScriptableObject.CreateInstance<ItemData>();
            _itemData.ID = (short)i;
            string saveFile = ReadFromFile($"{path}/{itemsLocation[i]}");
            JsonUtility.FromJsonOverwrite(saveFile, _itemData);

            //Checks if item is in standard's
            _allItems.Add(_itemData);
        }
    }
}