using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComparisonItem : MonoBehaviour
{
    public ItemID _itemID;

    public Image itemShowcase;
    public TMP_Text itemNameText;
    public TMP_Text holdingTypeText;

    public Transform statContent;
    public TMP_Text statTextPrefab;

    public void OverrideData()
    {
        ItemData _itemData = null;
        List<Attributes> _attributes = new();

        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                _itemData = _itemID._weaponItem._itemData;
                _attributes = _itemID._weaponItem._itemAttributes;

                holdingTypeText.text = "Holding type:" + _itemID._weaponItem.holdingType;

                for (int i = 0; i < _attributes.Count; i++)
                {
                    TMP_Text newStatText = Instantiate(statTextPrefab, statContent);
                    newStatText.text = $"{_attributes[i].attributeType}: {_attributes[i].amount}";
                }

                break;

            case ItemType.Armor:
                _itemData = _itemID._armorItem._itemData;
                _attributes = _itemID._armorItem._itemAttributes;

                holdingTypeText.text = "Holding type:" + _itemID._armorItem.armorType;

                for (int i = 0; i < _attributes.Count; i++)
                {
                    TMP_Text newStatText = Instantiate(statTextPrefab, statContent);
                    newStatText.text = $"{_attributes[i].attributeType}: {_attributes[i].amount}";
                }

                break;
        }

        itemShowcase.sprite = _itemData.itemSprite;
        itemNameText.text = _itemData.itemName;
    }
}
