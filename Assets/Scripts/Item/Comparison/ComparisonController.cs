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
                WeaponItem _weaponItem = _gearHolder.GetHoldingWeapon(_otherItemID._weaponItem.holdingType);

                //If weapon is found then return it
                if (_weaponItem != null)
                    return _weaponItem.GetComponent<ItemID>();
                break;

            case ItemType.Armor:
                ArmorItem _armorItem = _gearHolder.GetHoldingArmor(_otherItemID._armorItem.armorType);

                //If armor is found then return it
                if (_armorItem != null)
                    return _armorItem.GetComponent<ItemID>();
                break;
        }

        return null;
    }
}
