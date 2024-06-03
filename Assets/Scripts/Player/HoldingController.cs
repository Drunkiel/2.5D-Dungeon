using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Collectable,
}

public class HoldingController : MonoBehaviour
{
    public static HoldingController instance;
    public GearHolder _gearHolder;

    private void Awake()
    {
        instance = this;
    }

    public void PickItem(ItemID _itemID)
    {
        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                if (!CanPickWeapon(_itemID._weaponItem.holdingType))
                {
                    print($"Can't pick item: {_itemID.itemType}");
                    return;
                }

                switch (_itemID._weaponItem.holdingType)
                {
                    //Picking weapon to right hand
                    case WeaponHoldingType.Right_Hand:
                        _gearHolder._weaponRight = PickWeapon(_itemID, _gearHolder.rightHandTransform);
                        break;

                    //Picking weapon to left hand
                    case WeaponHoldingType.Left_Hand:
                        _gearHolder._weaponLeft = PickWeapon(_itemID, _gearHolder.leftHandTransform);
                        break;

                    //Picking weapon to both hands
                    case WeaponHoldingType.Both_Hands:
                        _gearHolder._weaponBoth = PickWeapon(_itemID, _gearHolder.bothHandTransform);
                        break;
                }
                break;

            case ItemType.Armor:
                if (!CanPickArmor(_itemID._armorItem.armorType))
                {
                    print($"Can't pick armor: {_itemID.itemType}");
                    return;
                }

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
                break;

            case ItemType.Collectable:
                break;
        }

        Destroy(_itemID.gameObject);
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

    public WeaponItem PickWeapon(ItemID _itemID, Transform newParent)
    {
        GameObject weaponCopy = Instantiate(_itemID.gameObject, newParent);
        weaponCopy.transform.localRotation = Quaternion.identity;
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
       // weaponCopy.transform.localRotation = Quaternion.identity; Maybe later
        weaponCopy.transform.localScale = Vector3.one;

        return weaponCopy.GetComponent<ItemID>()._armorItem;
    }
}
