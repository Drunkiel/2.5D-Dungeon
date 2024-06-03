using UnityEngine;

/*public enum ArmorHoldingType
{
    Head,
    Body,
    Feet,
}
*/
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
    //public ArmorHoldingType holdingType;
    public int protection;
    public int durability;
}
