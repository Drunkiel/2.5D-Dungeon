using UnityEngine;

public class ComparisonController : MonoBehaviour
{
    public ComparisonItem currentItem;
    public ComparisonItem otherItem;

    [SerializeField] private OpenCloseUI _openCloseUI;

    public void MakeComparison(ItemID _otherItemID)
    {
        otherItem._itemID = _otherItemID;
        currentItem._itemID = GetItem(_otherItemID);

        if (currentItem._itemID == null)
            return;
        else 
            _openCloseUI.OpenClose();

        currentItem.OverrideData();
        otherItem.OverrideData();
    }

    private ItemID GetItem(ItemID _otherItemID)
    {
        GearHolder _gearHolder = PlayerController.instance._holdingController._gearHolder;

        switch (_otherItemID.itemType)
        {
            case ItemType.Weapon:
                switch (_otherItemID._weaponItem.holdingType)
                {
                    case WeaponHoldingType.Right_Hand:
                        if (_gearHolder._weaponRight == null)
                            return null;

                        return _gearHolder._weaponRight.GetComponent<ItemID>();

                    case WeaponHoldingType.Left_Hand:
                        if (_gearHolder._weaponLeft == null)
                            return null;

                        return _gearHolder._weaponLeft.GetComponent<ItemID>();

                    case WeaponHoldingType.Both_Hands:
                        if (_gearHolder._weaponBoth == null)
                            return null;

                        return _gearHolder._weaponBoth.GetComponent<ItemID>();
                }
                break;

            case ItemType.Armor:
                switch (_otherItemID._armorItem.armorType)
                {
                    case ArmorType.Helmet:
                        if (_gearHolder._armorHead == null)
                            return null;

                        return _gearHolder._armorHead.GetComponent<ItemID>();

                    case ArmorType.Chestplate:
                        if (_gearHolder._armorChestplate == null)
                            return null;

                        return _gearHolder._armorChestplate.GetComponent<ItemID>();

                    case ArmorType.Boots:
                        if (_gearHolder._armorRightBoot == null)
                            return null;

                        return _gearHolder._armorRightBoot.GetComponent<ItemID>();
                }
                break;
        }

        return null;
    }
}
