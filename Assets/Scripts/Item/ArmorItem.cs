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
    public int protection;
    public int durability;
}
