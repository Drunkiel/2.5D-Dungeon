[System.Serializable]
public class InventoryData
{
    public int weaponRightID;
    public int weaponLeftID;

    public int armorHeadID;
    public int armorChestplateID;
    public int armorBootsID;

    public InventoryData() { }

    public InventoryData(GearHolder _gearHolder)
    {
        //Get weapons
        if (_gearHolder._weaponRight != null)
            weaponRightID = _gearHolder._weaponRight.GetComponent<ItemID>()._itemData.ID;
        if (_gearHolder._weaponLeft != null)
            weaponLeftID = _gearHolder._weaponLeft.GetComponent<ItemID>()._itemData.ID;

        //Get armor
        if (_gearHolder._armorHead != null)
            armorHeadID = _gearHolder._armorHead.GetComponent<ItemID>()._itemData.ID;
        if (_gearHolder._armorChestplate != null)
            armorChestplateID = _gearHolder._armorChestplate.GetComponent<ItemID>()._itemData.ID;
        if (_gearHolder._armorBoots != null)
            armorBootsID = _gearHolder._armorBoots.GetComponent<ItemID>()._itemData.ID;
    }
}
