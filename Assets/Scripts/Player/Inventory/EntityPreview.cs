using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EntityPreview 
{
    //Player body textures
    [Header("Head")]
    public Image headImage;

    [Header("Body")]
    public Image bodyImage;

    [Header("Arms")]
    public Image armLeftImage;
    public Image armRightImage;

    [Header("Hands")]
    public Image handLeftImage;
    public Image handRightImage;

    [Header("Legs")]
    public Image legLeftImage;
    public Image legRightImage;

    [Header("Armor")]
    [SerializeField] private Image helmetImage;
    [SerializeField] private Image chestplateImage;
    [SerializeField] private Image bootLeftImage;
    [SerializeField] private Image bootRightImage;

    public void UpdateEntityPreviewLook(EntityPartType partType, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case EntityPartType.Head:
                headImage.sprite = sprite;
                break;
            
            case EntityPartType.Body:
                bodyImage.sprite = sprite;
                break;
            
            case EntityPartType.Arms:
                armLeftImage.sprite = sprite;
                armRightImage.sprite = sprite;
                break;

            case EntityPartType.Hands:
                handLeftImage.sprite = sprite;
                handRightImage.sprite = sprite;
                break;

            case EntityPartType.Legs:
                legLeftImage.sprite = sprite;
                legRightImage.sprite = sprite;
                break;
        }
    }

    public void UpdateArmorLook(ArmorType partType, Sprite sprite)
    {
        switch (partType)
        {
            case ArmorType.Helmet:
                if (sprite != null)
                    helmetImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;

                helmetImage.sprite = sprite;
                break;
            
            case ArmorType.Chestplate:
                if (sprite != null)
                    chestplateImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;

                chestplateImage.sprite = sprite;
                break;

            case ArmorType.Boots:
                if (sprite != null)
                {
                    bootLeftImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;
                    bootRightImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;
                }

                bootLeftImage.sprite = sprite;
                bootRightImage.sprite = sprite;
                break;
        }
    }

    public void UpdateAllByEntity(EntityLook _entityLook, GearHolder _gearHolder)
    {   
        if (headImage != null)
            UpdateEntityPreviewLook(EntityPartType.Head, _entityLook.headImage.sprite);

        if (bodyImage != null)
            UpdateEntityPreviewLook(EntityPartType.Body, _entityLook.bodyImage.sprite);

        if (armLeftImage != null && armRightImage != null)
            UpdateEntityPreviewLook(EntityPartType.Arms, _entityLook.armLeftImage.sprite);

        if (handLeftImage != null && handRightImage != null)
            UpdateEntityPreviewLook(EntityPartType.Hands, _entityLook.handLeftImage.sprite);

        if (legLeftImage != null && legRightImage != null)
            UpdateEntityPreviewLook(EntityPartType.Legs, _entityLook.legLeftImage.sprite);

        //Armor
        if (helmetImage != null && _gearHolder._armorHead != null)
            UpdateArmorLook(ArmorType.Helmet, _gearHolder._armorHead.itemSprite);

        if (chestplateImage != null && _gearHolder._armorChestplate != null)
            UpdateArmorLook(ArmorType.Chestplate, _gearHolder._armorChestplate.itemSprite);

        if (bootLeftImage != null && bootRightImage != null && _gearHolder._armorBoots != null)
            UpdateArmorLook(ArmorType.Boots, _gearHolder._armorBoots.itemSprite);
    }
}
