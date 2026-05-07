using UnityEditor;
using UnityEngine;

public class ItemCreatorWindow : EditorWindow
{
    public ItemData _itemData;
    public WeaponData _weaponData;
    public ArmorData _armorData;
    public CollectableData _collectableData;
    public ItemType itemType;

    [MenuItem("Tools/Item Creator")]
    static void Open()
    {
        GetWindow<ItemCreatorWindow>("Item Creator");
    }

    void OnGUI()
    {
        _itemData = (ItemData)EditorGUILayout.ObjectField(
            "Item Data",
            _itemData,
            typeof(ItemData),
            false
        );

        _weaponData = (WeaponData)EditorGUILayout.ObjectField(
            "Weapon Data",
            _weaponData,
            typeof(WeaponData),
            false
        );

        _armorData = (ArmorData)EditorGUILayout.ObjectField(
            "Armor Data",
            _armorData,
            typeof(ArmorData),
            false
        );

        _collectableData = (CollectableData)EditorGUILayout.ObjectField(
            "Collectable Data",
            _collectableData,
            typeof(CollectableData),
            false
        );

        itemType = (ItemType)GUILayout.Toolbar(
            (int)itemType,
            new[] { "None", "Weapon", "Armor", "Collectable" }
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Make Json"))
        {
            if (_itemData == null)
                return;

            string itemName = _itemData.displayedName.Replace(" ", "");
            _itemData.name = itemName;

            switch (itemType)
            {
                case ItemType.None:
                    Debug.Log("None as option is selected in Item Creator");
                    break;

                case ItemType.Weapon:
                    if (_weaponData == null)
                        return;

                    _itemData.spritePathFront = $"{itemName}/Texture.png";
                    _itemData.spritePathBack = "";
                    _itemData.spriteIconPath = $"{itemName}/Icon.png";
                    _weaponData.name = itemName;
                    SaveLoadSystem.Ala(_weaponData, $"{SaveLoadSystem.itemsSavePath}{itemName}.json");
                    break;

                case ItemType.Armor:
                    if (_armorData == null)
                        return;

                    _itemData.spritePathFront = $"{_itemData.displayedName.Split(' ')[0]}/Sprites/{_armorData.armorType}_Front.png";
                    _itemData.spritePathBack = $"{_itemData.displayedName.Split(' ')[0]}/Sprites/{_armorData.armorType}_Back.png";
                    _itemData.spriteIconPath = $"{_itemData.displayedName.Split(' ')[0]}/Icons/Icon_{_armorData.armorType}.png";
                    _armorData.name = itemName;
                    SaveLoadSystem.Ala(_armorData, $"{SaveLoadSystem.itemsSavePath}{itemName}.json");
                    break;

                case ItemType.Collectable:
                    if (_collectableData == null)
                        return;

                    _itemData.spritePathFront = $"{itemName}/Icon.png";
                    _itemData.spritePathBack = "";
                    _itemData.spriteIconPath = $"{itemName}/Icon.png";
                    _collectableData.name = itemName;
                    SaveLoadSystem.Ala(_collectableData, $"{SaveLoadSystem.itemsSavePath}{itemName}.json");
                    break;
            }

            Debug.Log("Item created");
        }
    }
}