using UnityEngine;

public enum ArmorType
{
    Helmet,
    Chestplate,
    Boots,
}

public class ArmorItem : MonoBehaviour
{
    public ArmorType armorType;
    public Sprite itemSpriteFront;
    public Sprite itemSpriteBack;
    public Sprite iconSprite;
}
