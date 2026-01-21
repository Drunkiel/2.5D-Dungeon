using UnityEngine;

public class EntityItemLoader : MonoBehaviour
{
    public string weaponRightName;
    public string weaponLeftName;

    public string armorHeadName;
    public string armorChestplateName;
    public string armorBootsName;

    private ItemController _itemController;

    private void Start()
    {
        _itemController = GetComponent<ItemController>();
        SetItems();
    }

    public void SetItems()
    {
        ItemContainer _itemContainer = ItemContainer.instance;

        //Give weapon to right hand
        if (weaponRightName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByNameAndType(weaponRightName, ItemType.Weapon);
            if (_itemID != null)
                _itemController.SetWeapon(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no weapon called: {weaponRightName}");
        }

        //Give weapon to left hand
        if (weaponLeftName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByNameAndType(weaponLeftName, ItemType.Weapon);
            if (_itemID != null)
                _itemController.SetWeapon(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no weapon called: {weaponLeftName}");
        }

        //Give armor to head
        if (armorHeadName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByNameAndType(armorHeadName, ItemType.Armor);
            if (_itemID != null)
                _itemController.SetArmor(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no armor called: {armorHeadName}");
        }

        //Give armor to chestplate
        if (armorChestplateName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByNameAndType(armorChestplateName, ItemType.Armor);
            if (_itemID != null)
                _itemController.SetArmor(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no armor called: {armorChestplateName}");
        }

        //Give armor to boots
        if (armorBootsName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByNameAndType(armorBootsName, ItemType.Armor);
            if (_itemID != null)
                _itemController.SetArmor(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no armor called: {armorBootsName}");
        }
    }
}
