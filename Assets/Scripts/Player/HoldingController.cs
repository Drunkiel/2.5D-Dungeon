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

    public WeaponItem _weaponRight;
    public WeaponItem _weaponLeft;
    public WeaponItem _weaponBoth;

    private void Awake()
    {
        instance = this;
    }

    public void PickItem(ItemID _itemID)
    {
        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                if (CanPickWeapon(_itemID._weaponItem.holdingType))
                    print("a");
                break;
                /*            case ItemType.Armor:
                                break;
                            case ItemType.Collectable:
                                break;*/
        }
    }

    public bool CanPickWeapon(WeaponHoldingType holdingType)
    {
        switch (holdingType)
        {
            case WeaponHoldingType.Right_Hand:
                if (_weaponRight == null && _weaponBoth == null)
                    return true;
                else
                    return false;

            case WeaponHoldingType.Left_Hand:
                if (_weaponLeft == null && _weaponBoth == null)
                    return true;
                else
                    return false;

            case WeaponHoldingType.Both_Hands:
                if (_weaponBoth == null && _weaponLeft == null && _weaponRight == null)
                    return true;
                else
                    return false;
        }

        return false;
    }
}
