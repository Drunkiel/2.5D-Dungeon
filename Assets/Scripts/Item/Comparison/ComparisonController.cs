using UnityEngine;

public class ComparisonController : MonoBehaviour
{
    public static ComparisonController instance;

    public ComparisonItem currentItem;
    public ComparisonItem otherItem;

    [SerializeField] private OpenCloseInteraction _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void MakeComparison(ItemID _otherItemID, bool updateUI = true)
    {
        otherItem._itemID = _otherItemID;
        currentItem._itemID = GetItem(_otherItemID);

        if (currentItem._itemID == null)
            return;
        else if (updateUI) 
            _openCloseUI.Open(1);

        currentItem.OverrideData();
        otherItem.OverrideData();
    }

    private ItemID GetItem(ItemID _otherItemID)
    {
        GearHolder _gearHolder = PlayerController.instance._holdingController._itemController._gearHolder;

        switch (_otherItemID._itemData.itemType)
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
                        if (_gearHolder._armorBoots == null)
                            return null;

                        return _gearHolder._armorBoots.GetComponent<ItemID>();
                }
                break;
        }

        return null;
    }
}
