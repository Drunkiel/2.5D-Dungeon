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
    public List<TMP_Text> allContextTexts = new();

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

                break;

            case ItemType.Armor:
                _itemData = _itemID._armorItem._itemData;
                _attributes = _itemID._armorItem._itemAttributes;

                holdingTypeText.text = "Holding type:" + _itemID._armorItem.armorType;

                break;
        }

        //Removing content
        for (int i = 0; i < allContextTexts.Count; i++)
        {
            Destroy(allContextTexts[i].gameObject);
        }
        allContextTexts.Clear();

        //Adding new content
        for (int i = 0; i < _attributes.Count; i++)
        {
            TMP_Text newStatText = Instantiate(statTextPrefab, statContent);
            newStatText.text = $"{_attributes[i].attributeType}: {_attributes[i].amount}";
            allContextTexts.Add(newStatText);
        }

        itemShowcase.sprite = _itemData.itemSprite;
        itemNameText.text = _itemData.itemName;
    }
}
