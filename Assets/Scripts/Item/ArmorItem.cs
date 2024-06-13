using System.Collections.Generic;
using UnityEngine;

public enum ArmorType
{
    Helmet,
    Chestplate,
    Boots,
}

public class ArmorItem : MonoBehaviour
{
    public ItemData _itemData;
    public ArmorType armorType;

    public List<Attributes> _itemAttributes = new();

    private void Start()
    {
        //Overwritting sprite
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = _itemData.itemSprite;
    }
}
