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

    private void Start()
    {
        //Overwriting sprite
        //transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = GetComponent<ItemID>()._itemData.sprite;
    }
}
