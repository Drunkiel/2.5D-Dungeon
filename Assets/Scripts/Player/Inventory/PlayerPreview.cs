using System;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerPartType
{
    Head,
    Body,
    Feet
}

[Serializable]
public class PlayerPreview 
{
    //Player body textures
    [SerializeField] private Image headImage;
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image feetLeftImage;
    [SerializeField] private Image feetRightImage;

    //Armor textures
    [SerializeField] private Image helmetImage;
    [SerializeField] private Image chestplateImage;
    [SerializeField] private Image bootLeftImage;
    [SerializeField] private Image bootRightImage;

    public void UpdatePlayerLook(PlayerPartType partType, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case PlayerPartType.Head:
                headImage.sprite = sprite;
                break;
            
            case PlayerPartType.Body:
                bodyImage.sprite = sprite;
                break;

            case PlayerPartType.Feet:
                feetLeftImage.sprite = sprite;
                feetRightImage.sprite = sprite;
                break;
        }
    }

    public void UpdateArmorLook(ArmorType partType, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case ArmorType.Helmet:
                helmetImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;

                helmetImage.sprite = sprite;
                break;
            
            case ArmorType.Chestplate:
                chestplateImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;

                chestplateImage.sprite = sprite;
                break;

            case ArmorType.Boots:
                bootLeftImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;
                bootRightImage.GetComponent<RectTransform>().sizeDelta = sprite.rect.size / 100;

                bootLeftImage.sprite = sprite;
                bootRightImage.sprite = sprite;
                break;
        }
    }
}
