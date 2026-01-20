using UnityEngine;

public class EntityItemLoader : MonoBehaviour
{
    public string weaponRightName;
    public string weaponLeftName;
    public string weaponBothName;

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
            ItemID _itemID = _itemContainer.GetItemByName(weaponRightName);
            if (_itemID != null)
                _itemController.SetWeapon(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no weapon called: {weaponRightName}");
        }

        //Give weapon to left hand
        if (weaponLeftName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByName(weaponLeftName);
            if (_itemID != null)
                _itemController.SetWeapon(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no weapon called: {weaponLeftName}");
        }

        //Give weapon to both hands
        if (weaponBothName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByName(weaponBothName);
            if (_itemID != null)
                _itemController.SetWeapon(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no weapon called: {weaponBothName}");
        }

        //Give armor to head
        if (armorHeadName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByName(armorHeadName);
            if (_itemID != null)
                _itemController.SetArmor(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no armor called: {armorHeadName}");
        }

        //Give armor to chestplate
        if (armorChestplateName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByName(armorChestplateName);
            if (_itemID != null)
                _itemController.SetArmor(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no armor called: {armorChestplateName}");
        }

        //Give armor to boots
        if (armorBootsName.Length != 0)
        {
            ItemID _itemID = _itemContainer.GetItemByName(armorBootsName);
            if (_itemID != null)
                _itemController.SetArmor(_itemID);
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"There is no armor called: {armorBootsName}");
        }
    }
}
