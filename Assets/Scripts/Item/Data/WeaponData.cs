using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Weapon data")]
public class WeaponData : ScriptableObject
{
    public ItemData _itemData;
    public WeaponType weaponType;
    public WeaponHoldingType holdingType;
    public bool resizable = true;
}
