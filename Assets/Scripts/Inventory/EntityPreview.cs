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

    public void UpdateEntityPreviewLook(EntityPartType partType, Vector3 position, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case EntityPartType.Head:
                UpdateSingleImage(headImage, position, sprite);
                break;

            case EntityPartType.Body:
                UpdateSingleImage(bodyImage, position, sprite);
                break;

            case EntityPartType.Arms:
                UpdateSymmetricalParts(new[] { armLeftImage, armRightImage }, position, sprite);
                break;

            case EntityPartType.Hands:
                UpdateHands(handLeftImage, handRightImage, position, sprite);
                break;

            case EntityPartType.Legs:
                UpdateSymmetricalParts(new[] { legLeftImage, legRightImage }, position, sprite);
                break;
        }
    }

    private void UpdateSingleImage(Image image, Vector3 position, Sprite sprite)
    {
        image.sprite = sprite;
        image.transform.localPosition = position;

        image.SetNativeSize();
        image.rectTransform.sizeDelta /= 100f;
    }

    private void UpdateSymmetricalParts(Image[] images, Vector3 position, Sprite sprite)
    {
        for (int i = 0; i < images.Length; i++)
        {
            Image img = images[i];
            img.sprite = sprite;

            img.transform.localPosition = new Vector3(0, position.y, img.transform.localPosition.z);

            float xOffset = i == 0 ? position.x : -position.x;
            img.transform.parent.localPosition = new Vector3(xOffset, position.z, img.transform.parent.localPosition.z);

            img.SetNativeSize();
            img.rectTransform.sizeDelta /= 100f;
        }
    }

    private void UpdateHands(Image leftHand, Image rightHand, Vector3 position, Sprite sprite)
    {
        leftHand.sprite = sprite;
        rightHand.sprite = sprite;

        leftHand.transform.localPosition = position;
        rightHand.transform.localPosition = new Vector3(-position.x, position.y, position.z);

        leftHand.SetNativeSize();
        rightHand.SetNativeSize();

        leftHand.rectTransform.sizeDelta /= 100f;
        rightHand.rectTransform.sizeDelta /= 100f;
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

    public void UpdateAllByEntity(EntityLook _entityLook, EntitySpriteHolder _spriteHolder, GearHolder _gearHolder)
    {
        if (headImage != null)
            UpdateEntityPreviewLook(EntityPartType.Head, _entityLook.headImage.transform.localPosition, _spriteHolder.head_Front);

        if (bodyImage != null)
            UpdateEntityPreviewLook(EntityPartType.Body, _entityLook.bodyImage.transform.localPosition, _spriteHolder.body_Front);

        if (armLeftImage != null && armRightImage != null)
            UpdateEntityPreviewLook(EntityPartType.Arms, new(_entityLook.armLeftImage.transform.parent.localPosition.x,
                                                             _entityLook.armLeftImage.transform.localPosition.y, _entityLook.armLeftImage.transform.parent.parent.localPosition.y), _spriteHolder.arm_Front);

        if (handLeftImage != null && handRightImage != null)
            UpdateEntityPreviewLook(EntityPartType.Hands, _entityLook.handLeftImage.transform.localPosition, _spriteHolder.hand_Front);

        if (legLeftImage != null && legRightImage != null)
            UpdateEntityPreviewLook(EntityPartType.Legs, new(_entityLook.legLeftImage.transform.parent.localPosition.x,
                                                             _entityLook.legLeftImage.transform.localPosition.y, -0.055f), _spriteHolder.leg_Front);

        //Armor
        if (helmetImage != null)
            UpdateArmorLook(
                ArmorType.Helmet,
                _gearHolder._armorHead == null ?
                placeholderSprite :
                _gearHolder._armorHead.itemSpriteFront
            );

        if (chestplateImage != null)
            UpdateArmorLook(
                ArmorType.Chestplate,
                _gearHolder._armorChestplate == null ?
                placeholderSprite :
                _gearHolder._armorChestplate.itemSpriteFront
            );

        if (bootLeftImage != null && bootRightImage != null)
            UpdateArmorLook(
                ArmorType.Boots,
                _gearHolder._armorBoots == null ?
                placeholderSprite :
                _gearHolder._armorBoots.itemSpriteFront
            );
    }
}
