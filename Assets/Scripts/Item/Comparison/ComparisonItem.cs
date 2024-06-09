using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComparisonItem : MonoBehaviour
{
    public ItemID _itemID;

    public Image itemShowcase;
    public TMP_Text itemNameText;
    public TMP_Text holdingTypeText;

    public GameObject[] itemTypeObjects;
    public TMP_Text[] idkTexts;

    public void OverrideData()
    {
        ItemData _itemData = null;

        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                _itemData = _itemID._weaponItem._itemData;

                itemTypeObjects[0].SetActive(true);
                itemTypeObjects[1].SetActive(false);

                holdingTypeText.text = "Holding type:" + _itemID._weaponItem.holdingType;
                idkTexts[0].text = "Damage: " + _itemID._weaponItem.damage;
                idkTexts[1].text = "No mana so far";
                idkTexts[2].text = "Durability: " + _itemID._weaponItem.durability;
                break;

            case ItemType.Armor:
                _itemData = _itemID._armorItem._itemData;

                itemTypeObjects[0].SetActive(false);
                itemTypeObjects[1].SetActive(true);

                holdingTypeText.text = "Holding type:" + _itemID._armorItem.armorType;
                idkTexts[3].text = "Protection: " + _itemID._armorItem.protection;
                idkTexts[4].text = "Durability: " + _itemID._armorItem.durability;
                break;
        }

        itemShowcase.sprite = _itemData.itemSprite;
        itemNameText.text = _itemData.itemName;
    }
}
