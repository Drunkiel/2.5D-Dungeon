using System.Collections.Generic;
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
    Wand,
}

public class WeaponItem : MonoBehaviour
{
    public ItemData _itemData;
    public WeaponType weaponType;
    public WeaponHoldingType holdingType;
    public bool resizable = true;

    private void Start()
    {
        //Overwriting sprite
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = _itemData.sprite;
    }
}
