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

    [SerializeField] private Sprite placeholderSprite;

    public void UpdateEntityPreviewLook(EntityPartType partType, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case EntityPartType.Head:
                headImage.sprite = sprite;
                headImage.SetNativeSize();
                headImage.rectTransform.sizeDelta = new Vector2(headImage.rectTransform.sizeDelta.x / 100, headImage.rectTransform.sizeDelta.y / 100);
                break;
            
            case EntityPartType.Body:
                bodyImage.sprite = sprite;
                bodyImage.SetNativeSize();
                bodyImage.rectTransform.sizeDelta = new Vector2(bodyImage.rectTransform.sizeDelta.x / 100, bodyImage.rectTransform.sizeDelta.y / 100);
                break;
            
            case EntityPartType.Arms:
                armLeftImage.sprite = sprite;
                armRightImage.sprite = sprite;

                armLeftImage.SetNativeSize();
                armLeftImage.rectTransform.sizeDelta = new Vector2(armLeftImage.rectTransform.sizeDelta.x / 100, armLeftImage.rectTransform.sizeDelta.y / 100);
                armRightImage.SetNativeSize();
                armRightImage.rectTransform.sizeDelta = new Vector2(armRightImage.rectTransform.sizeDelta.x / 100, armRightImage.rectTransform.sizeDelta.y / 100);
                break;

            case EntityPartType.Hands:
                handLeftImage.sprite = sprite;
                handRightImage.sprite = sprite;

                handLeftImage.SetNativeSize();
                handLeftImage.rectTransform.sizeDelta = new Vector2(handLeftImage.rectTransform.sizeDelta.x / 100, handLeftImage.rectTransform.sizeDelta.y / 100);
                handRightImage.SetNativeSize();
                handRightImage.rectTransform.sizeDelta = new Vector2(handRightImage.rectTransform.sizeDelta.x / 100, handRightImage.rectTransform.sizeDelta.y / 100);
                break;

            case EntityPartType.Legs:
                legLeftImage.sprite = sprite;
                legRightImage.sprite = sprite;

                legLeftImage.SetNativeSize();
                legLeftImage.rectTransform.sizeDelta = new Vector2(legLeftImage.rectTransform.sizeDelta.x / 100, legLeftImage.rectTransform.sizeDelta.y / 100);
                legRightImage.SetNativeSize();
                legRightImage.rectTransform.sizeDelta = new Vector2(legRightImage.rectTransform.sizeDelta.x / 100, legRightImage.rectTransform.sizeDelta.y / 100);
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
        if (helmetImage != null)
            UpdateArmorLook(ArmorType.Helmet, _gearHolder._armorHead == null ? placeholderSprite : _gearHolder._armorHead.itemSprite);

        if (chestplateImage != null)
            UpdateArmorLook(ArmorType.Chestplate, _gearHolder._armorChestplate == null ? placeholderSprite : _gearHolder._armorChestplate.itemSprite);

        if (bootLeftImage != null && bootRightImage != null)
            UpdateArmorLook(ArmorType.Boots, _gearHolder._armorBoots == null ? placeholderSprite : _gearHolder._armorBoots.itemSprite);
    }
}
