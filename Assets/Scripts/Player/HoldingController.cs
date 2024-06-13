using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Collectable,
}

public class HoldingController : MonoBehaviour
{
    public GearHolder _gearHolder;

    public bool PickItem(ItemID _itemID)
    {
        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                if (!CanPickWeapon(_itemID._weaponItem.holdingType))
                {
                    ReplaceItem(_itemID);
                    return false;
                }

                SetWeapon(_itemID);
                break;

            case ItemType.Armor:
                if (!CanPickArmor(_itemID._armorItem.armorType))
                {
                    ReplaceItem(_itemID);
                    return false;
                }

                SetArmor(_itemID);
                break;

            case ItemType.Collectable:
                InventoryController _inventoryController = InventoryController.instance;

                //Looking for available slot in inventory
                int availableSlot = _inventoryController.GetAvailableSlotIndex();
                if (availableSlot == -1)
                    return false;

                //Cloning item to founded slot and adding it to inventory
                GameObject itemClone = Instantiate(_itemID.gameObject, _inventoryController._slots[availableSlot].transform);
                _inventoryController.AddToInventory(itemClone.GetComponent<ItemID>(), availableSlot);
                break;
        }

        Destroy(_itemID.gameObject);
        return true;
    }

    public bool CanPickWeapon(WeaponHoldingType holdingType)
    {
        switch (holdingType)
        {
            case WeaponHoldingType.Right_Hand:
                if (_gearHolder._weaponRight == null && _gearHolder._weaponBoth == null)
                    return true;
                else
                    return false;

            case WeaponHoldingType.Left_Hand:
                if (_gearHolder._weaponLeft == null && _gearHolder._weaponBoth == null)
                    return true;
                else
                    return false;

            case WeaponHoldingType.Both_Hands:
                if (_gearHolder._weaponBoth == null && _gearHolder._weaponLeft == null && _gearHolder._weaponRight == null)
                    return true;
                else
                    return false;
        }

        return false;
    }

    public WeaponItem PickWeapon(ItemID _itemID, Quaternion rotation, Transform newParent)
    {
        GameObject weaponCopy = Instantiate(_itemID.gameObject, newParent);
        weaponCopy.name = _itemID.name;
        weaponCopy.transform.localRotation = rotation;
        if (_itemID._weaponItem.resizable)
            weaponCopy.transform.localScale = Vector3.one;

        return weaponCopy.GetComponent<ItemID>()._weaponItem;
    }

    public bool CanPickArmor(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Helmet:
                if (_gearHolder._armorHead == null)
                    return true;
                else
                    return false;

            case ArmorType.Chestplate:
                if (_gearHolder._armorChestplate == null)
                    return true;
                else
                    return false;

            case ArmorType.Boots:
                if (_gearHolder._armorBoots == null)
                    return true;
                else
                    return false;
        }

        return false;
    }

    public ArmorItem PickArmor(ItemID _itemID, Transform newParent)
    {
        GameObject armorCopy = Instantiate(_itemID.gameObject, newParent);
        armorCopy.name = _itemID.name;
        armorCopy.transform.localScale = Vector3.one;

        return armorCopy.GetComponent<ItemID>()._armorItem;
    }

    public void ReplaceItem(ItemID _itemID)
    {
        if (_itemID == null)
            return;

        ItemID _holdingItemID;
        PickInteraction _pickInteraction = null;

        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                //Get current item
                WeaponItem _weaponItem = _gearHolder.GetHoldingWeapon(_itemID._weaponItem.holdingType);

                //Checks if item is found
                if (_weaponItem == null)
                    return;

                //Assign founded item 
                _holdingItemID = _weaponItem.GetComponent<ItemID>();

                //Gets stand Pick interaction script
                _pickInteraction = _itemID.transform.parent.parent.GetChild(0).GetComponent<PickInteraction>();

                //Making clone of weapon item and assigning it to stand
                Transform weaponClone = PickWeapon(_holdingItemID, Quaternion.identity, _itemID.transform.parent).gameObject.transform;
                if (weaponClone.GetComponent<ItemID>()._weaponItem.resizable)
                    weaponClone.localScale = new(0.25f, 0.25f, 0.25f);

                _pickInteraction._itemID = weaponClone.GetComponent<ItemID>();
                Destroy(_holdingItemID.gameObject);

                //Picking weapon by player
                SetWeapon(_itemID);
                break;

            case ItemType.Armor:
                //Get current item
                ArmorItem _armorItem = _gearHolder.GetHoldingArmor(_itemID._armorItem.armorType);

                //Checks if item is found
                if (_armorItem == null)
                    return;

                //Assign founded item 
                _holdingItemID = _armorItem.GetComponent<ItemID>();

                //Gets stand Pick interaction script
                _pickInteraction = _itemID.transform.parent.parent.GetChild(0).GetComponent<PickInteraction>();

                //Making clone of armor item and assigning it to stand
                Transform armorClone = PickArmor(_holdingItemID, _itemID.transform.parent).gameObject.transform;
                armorClone.localScale = new(0.5f, 0.5f, 0.5f);
                _pickInteraction._itemID = armorClone.GetComponent<ItemID>();

                //If armor is boots then destroy both of boots
                if (_holdingItemID._armorItem.armorType == ArmorType.Boots)
                    Destroy(_gearHolder.leftFeetTransform.GetChild(1).gameObject);
                Destroy(_holdingItemID.gameObject);


                //Picking armor by player
                SetArmor(_itemID);
                break;
        }

        ComparisonController.instance.MakeComparison(_pickInteraction._itemID, false);
        Destroy(_itemID.gameObject);
    }

    private void SetWeapon(ItemID _itemID)
    {
        switch (_itemID._weaponItem.holdingType)
        {
            //Picking weapon to right hand
            case WeaponHoldingType.Right_Hand:
                _gearHolder._weaponRight = PickWeapon(_itemID, Quaternion.Euler(0, 0, -90), _gearHolder.rightHandTransform);
                break;

            //Picking weapon to left hand
            case WeaponHoldingType.Left_Hand:
                Quaternion rotation = Quaternion.Euler(0, 0, -90);

                if (_itemID._weaponItem.weaponType == WeaponType.Shield)
                    rotation = Quaternion.identity;

                _gearHolder._weaponLeft = PickWeapon(_itemID, rotation, _gearHolder.leftHandTransform);
                break;

            //Picking weapon to both hands
            case WeaponHoldingType.Both_Hands:
                _gearHolder._weaponBoth = PickWeapon(_itemID, Quaternion.Euler(0, 0, -90), _gearHolder.bothHandTransform);
                break;
        }
    }

    private void SetArmor(ItemID _itemID)
    {
        switch (_itemID._armorItem.armorType)
        {
            //Picking armor to head
            case ArmorType.Helmet:
                _gearHolder._armorHead = PickArmor(_itemID, _gearHolder.headTransform);
                break;

            //Picking armor to body
            case ArmorType.Chestplate:
                _gearHolder._armorChestplate = PickArmor(_itemID, _gearHolder.bodyTransform);
                break;

            //Picking armor to legs
            case ArmorType.Boots:
                _gearHolder._armorBoots = PickArmor(_itemID, _gearHolder.rightFeetTransform);
                PickArmor(_itemID, _gearHolder.leftFeetTransform);
                break;
        }
    }
}
