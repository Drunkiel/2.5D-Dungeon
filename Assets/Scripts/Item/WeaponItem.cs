using UnityEngine;

public enum WeaponHoldingType
{
    Right_Hand,
    Left_Hand,
    Both_Hands,
}

public enum WeaponType
{
    Sword,
    Shield,
    Bow,
    Staff,
}

public class WeaponItem : MonoBehaviour
{
    public ItemData _itemData;
    public WeaponType weaponType;
    public WeaponHoldingType holdingType;
    public int damage;
    public int durability;
}
