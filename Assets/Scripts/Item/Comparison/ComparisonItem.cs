using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComparisonItem : MonoBehaviour
{
    public ItemID _itemID;

    [SerializeField] private Image itemShowcase;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text holdingTypeText;

    [SerializeField] private Transform statContent;
    [SerializeField] private TMP_Text statTextPrefab;
    private Sprite iconSprite;
    private string titleText;

    public void OverrideData()
    {
        if (_itemID._itemData != null)
            SetItemData();
        else
            SetSkillData();

        itemShowcase.sprite = iconSprite;
        itemNameText.text = titleText;
    }

    private void SetItemData()
    {
        ItemData _itemData = _itemID._itemData;
        List<ItemBuff> _itemBuffs = new();
        titleText = _itemData.displayedName;

        switch (_itemID._itemData.itemType)
        {
            case ItemType.Weapon:
                iconSprite = _itemID._weaponItem.iconSprite;

                holdingTypeText.text = "Holding type:" + _itemID._weaponItem.holdingType;
                break;

            case ItemType.Armor:
                iconSprite = _itemID._armorItem.iconSprite;

                holdingTypeText.text = "Holding type:" + _itemID._armorItem.armorType;
                break;
        }

        if (_itemData._itemBuffs.Count > 0)
            _itemBuffs = _itemData._itemBuffs;

        //Displaying stats
        for (int i = 0; i < _itemBuffs.Count; i++)
        {
            TMP_Text newStatText = Instantiate(statTextPrefab, statContent);
            newStatText.text = $"{_itemBuffs[i].itemBuffs}: {_itemBuffs[i].amount}";
        }
    }

    private void SetSkillData()
    {
        SkillDataParser _skillData = _itemID._skillDataParser;
        List<Attributes> _attributes = new();
        titleText = _skillData._skillData.displayedName;
        iconSprite = _skillData.iconSprite;


        if (_skillData._skillData._skillAttributes.Count > 0)
            _attributes = _skillData._skillData._skillAttributes;

        //Displaying stats
        for (int i = 0; i < _attributes.Count; i++)
        {
            string attributeName = BetterSkillNames(_attributes[i].attributeType, _attributes[i].buffTypes);

            TMP_Text newStatText = Instantiate(statTextPrefab, statContent);
            newStatText.text = $"{attributeName}: {_attributes[i].amount}";
        }
    }

    private string BetterSkillNames(AttributeTypes attribute, Buffs buff = Buffs.None)
    {
        return attribute switch
        {
            AttributeTypes.Buff => BetterBuffNames(buff),
            AttributeTypes.MeleeDamage => "Melee Damage",
            AttributeTypes.RangeDamage => "Range Damage",
            AttributeTypes.MagicDamage => "Magic Damage",
            AttributeTypes.AllProtection => "Protection",
            AttributeTypes.MeleeProtection => "Melee Protection",
            AttributeTypes.RangeProtection => "Range Protection",
            AttributeTypes.MagicProtection => "Magic Protection",
            AttributeTypes.Cooldown => "Cooldown",
            AttributeTypes.ManaUsage => "Mana Needed",
            _ => "",
        };
    }

    private string BetterBuffNames(Buffs buff)
    {
        return buff switch
        {
            Buffs.None => "",
            Buffs.MaxHealth => "+MaxHP",
            Buffs.MaxMana => "+MaxMP",
            Buffs.Damage => "+Damage",
            Buffs.Protection => "+Protection",
            Buffs.MaxSpeed => "+Speed",
            _ => "",
        };
    }
}
