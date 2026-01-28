using System;
using UnityEngine;

public enum ItemType
{
    None,
    Weapon,
    Armor,
    Collectable,
    Spell,
}

public class ItemController : SaveLoadSystem
{
    public GearHolder _gearHolder;

    public bool PickItem(ItemID _itemID, bool isPlayer = true)
    {
        //If npc then do nothing
        if (!isPlayer || _itemID == null)
            return false;

        InventoryController _inventoryController = InventoryController.instance;

        //Looking for available slot in inventory
        int availableSlot = _inventoryController.GetAvailableSlotIndex();
        if (availableSlot == -1)
            return false;

        //Cloning item to founded slot and adding it to inventory
        GameObject itemClone = Instantiate(_itemID.gameObject, _inventoryController._inventorySlots[availableSlot].transform);
        _inventoryController.AddToInventory(itemClone.GetComponent<ItemID>(), availableSlot);

        if (isPlayer)
            ConsoleController.instance.ChatMessage(SenderType.Hidden, "Picked");

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
        weaponCopy.transform.SetLocalPositionAndRotation(Vector3.zero, rotation);
        if (_itemID._weaponItem.resizable)
            weaponCopy.transform.localScale = new(0.5f, 0.5f, 1);

        if (_itemID._weaponItem.weaponType == WeaponType.Sword)
            weaponCopy.transform.localPosition = new(0.45f, 0, 0);

        GetComponent<EntityController>()._entityInfo.entityClass = _itemID._itemData.entityClass;
        return weaponCopy.GetComponent<ItemID>()._weaponItem;
    }

    public void SetWeapon(ItemID _itemID)
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
                _gearHolder._weaponLeft.transform.localScale = new(-0.5f, 0.5f, 1);
                break;

            //Picking weapon to both hands
            case WeaponHoldingType.Both_Hands:
                _gearHolder._weaponBoth = PickWeapon(_itemID, Quaternion.Euler(0, 0, -90), _gearHolder.bothHandTransform);
                break;
        }

        EntityController _playerController = GameController.instance._player;
        _playerController.GetComponent<EntityLookController>().RotateCharacter(!_playerController.isFlipped, _playerController.isFacingCamera);
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
        if (_itemID._armorItem.armorType != ArmorType.Boots)
            armorCopy.transform.localPosition = Vector3.zero;
        else
            armorCopy.transform.localPosition = new(0, -0.17f, 0);

        return armorCopy.GetComponent<ItemID>()._armorItem;
    }

    public void SetArmor(ItemID _itemID)
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

        EntityController _playerController = GameController.instance._player;
        _playerController.GetComponent<EntityLookController>().RotateCharacter(!_playerController.isFlipped, _playerController.isFacingCamera);
    }
}