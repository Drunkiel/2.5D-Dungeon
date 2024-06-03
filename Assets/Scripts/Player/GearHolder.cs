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

    [Header("Currently holded Items")]
    public WeaponItem _weaponRight;
    public WeaponItem _weaponLeft;
    public WeaponItem _weaponBoth;

    public ArmorItem _armorHead;
    public ArmorItem _armorChestplate;
    public ArmorItem _armorRightBoot;
    public ArmorItem _armorLeftBoot;
}
