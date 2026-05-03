using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GearHolder
{
    [Header("Parents of holding Items")]
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform bothHandTransform;

    public Transform headTransform;
    public Transform bodyTransform;
    public Transform rightFeetTransform;
    public Transform leftFeetTransform;

    [Header("Currently holden Items")]
    public WeaponItem _weaponRight;
    public WeaponItem _weaponLeft;
    public WeaponItem _weaponBoth;

    public ArmorItem _armorHead;
    public ArmorItem _armorChestplate;
    public ArmorItem _armorBoots;

    public WeaponItem GetHoldingWeapon(WeaponHoldingType holdingType)
    {
        return holdingType switch
        {
            WeaponHoldingType.Right_Hand => _weaponRight,
            WeaponHoldingType.Left_Hand => _weaponLeft,
            WeaponHoldingType.Both_Hands => _weaponBoth,
            _ => null,
        };
    }

    public Transform GetHoldingWeaponParent(WeaponHoldingType holdingType)
    {
        return holdingType switch
        {
            WeaponHoldingType.Right_Hand => rightHandTransform,
            WeaponHoldingType.Left_Hand => leftHandTransform,
            WeaponHoldingType.Both_Hands => bothHandTransform,
            _ => null,
        };
    }

    public ArmorItem GetHoldingArmor(ArmorType holdingType)
    {
        return holdingType switch
        {
            ArmorType.Helmet => _armorHead,
            ArmorType.Chestplate => _armorChestplate,
            ArmorType.Boots => _armorBoots,
            _ => null,
        };
    }

    public List<Attributes> GetAllAttributes()
    {
        List<Attributes> _allAttributes = new();

        if (_weaponLeft != null)
            _allAttributes.AddRange(_weaponLeft.GetComponent<ItemID>()._itemData._attributes);

        if (_weaponRight != null)
            _allAttributes.AddRange(_weaponRight.GetComponent<ItemID>()._itemData._attributes);

        if (_weaponBoth != null)
            _allAttributes.AddRange(_weaponBoth.GetComponent<ItemID>()._itemData._attributes);

        if (_armorHead != null)
            _allAttributes.AddRange(_armorHead.GetComponent<ItemID>()._itemData._attributes);

        if (_armorChestplate != null)
            _allAttributes.AddRange(_armorChestplate.GetComponent<ItemID>()._itemData._attributes);

        if (_armorBoots != null)
            _allAttributes.AddRange(_armorBoots.GetComponent<ItemID>()._itemData._attributes);

        return _allAttributes;
    }

    public List<ItemBuff> GetAllBuffs()
    {
        List<ItemBuff> _allItemBuffs = new();

        if (_weaponLeft != null)
            _allItemBuffs.AddRange(_weaponLeft.GetComponent<ItemID>()._itemData._itemBuffs);

        if (_weaponRight != null)
            _allItemBuffs.AddRange(_weaponRight.GetComponent<ItemID>()._itemData._itemBuffs);

        if (_weaponBoth != null)
            _allItemBuffs.AddRange(_weaponBoth.GetComponent<ItemID>()._itemData._itemBuffs);

        if (_armorHead != null)
            _allItemBuffs.AddRange(_armorHead.GetComponent<ItemID>()._itemData._itemBuffs);

        if (_armorChestplate != null)
            _allItemBuffs.AddRange(_armorChestplate.GetComponent<ItemID>()._itemData._itemBuffs);

        if (_armorBoots != null)
            _allItemBuffs.AddRange(_armorBoots.GetComponent<ItemID>()._itemData._itemBuffs);

        return _allItemBuffs;
    }
}
