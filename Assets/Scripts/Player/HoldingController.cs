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
                    print($"Can't pick item: {_itemID.itemType}");
                    ReplaceItem(_itemID);
                    return false;
                }

                SetWeapon(_itemID);
                break;

            case ItemType.Armor:
                if (!CanPickArmor(_itemID._armorItem.armorType))
                {
                    print($"Can't pick armor: {_itemID.itemType}");
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
        weaponCopy.transform.localRotation = rotation;
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
                if (_gearHolder._armorRightBoot == null && _gearHolder._armorLeftBoot == null)
                    return true;
                else
                    return false;
        }

        return false;
    }

    public ArmorItem PickArmor(ItemID _itemID, Transform newParent)
    {
        GameObject weaponCopy = Instantiate(_itemID.gameObject, newParent);
        weaponCopy.transform.localScale = Vector3.one;

        return weaponCopy.GetComponent<ItemID>()._armorItem;
    }

    public void ReplaceItem(ItemID _itemID)
    {
        ItemID _holdingItemID;

        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                //Replacing item on stand
                _holdingItemID = _gearHolder.GetHoldingWeapon(_itemID._weaponItem.holdingType).GetComponent<ItemID>();
                Transform weaponClone = PickWeapon(_holdingItemID, Quaternion.Euler(0, 0, 90), _itemID.transform.parent).gameObject.transform;
                weaponClone.localScale = new(0.25f, 0.25f, 0.25f);
                Destroy(_holdingItemID.gameObject);

                //Picking weapon by player
                SetWeapon(_itemID);

                Destroy(_itemID.gameObject);
                break;

            case ItemType.Armor:
                _holdingItemID = _gearHolder.GetHoldingArmor(_itemID._armorItem.armorType).GetComponent<ItemID>();
                break;
        }
    }

    private void SetWeapon(ItemID _itemID)
    {
        switch (_itemID._weaponItem.holdingType)
        {
            //Picking weapon to right hand
            case WeaponHoldingType.Right_Hand:
                _gearHolder._weaponRight = PickWeapon(_itemID, Quaternion.identity, _gearHolder.rightHandTransform);
                break;

            //Picking weapon to left hand
            case WeaponHoldingType.Left_Hand:
                _gearHolder._weaponLeft = PickWeapon(_itemID, Quaternion.identity, _gearHolder.leftHandTransform);
                break;

            //Picking weapon to both hands
            case WeaponHoldingType.Both_Hands:
                _gearHolder._weaponBoth = PickWeapon(_itemID, Quaternion.identity, _gearHolder.bothHandTransform);
                break;
        }
    }

    private void SetArmor(ItemID _itemID)
    {
        switch (_itemID._armorItem.armorType)
        {
            case ArmorType.Helmet:
                _gearHolder._armorHead = PickArmor(_itemID, _gearHolder.headTransform);
                break;

            case ArmorType.Chestplate:
                _gearHolder._armorChestplate = PickArmor(_itemID, _gearHolder.bodyTransform);
                break;

            case ArmorType.Boots:
                _gearHolder._armorRightBoot = PickArmor(_itemID, _gearHolder.rightFeetTransform);
                _gearHolder._armorLeftBoot = PickArmor(_itemID, _gearHolder.leftFeetTransform);
                break;
        }
    }
}
