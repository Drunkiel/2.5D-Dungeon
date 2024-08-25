using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerPreview 
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

    public void UpdatePlayerPreviewLook(PlayerPartType partType, Sprite sprite)
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
            
            case PlayerPartType.Arms:
                armLeftImage.sprite = sprite;
                armRightImage.sprite = sprite;
                break;

            case PlayerPartType.Hands:
                handLeftImage.sprite = sprite;
                handRightImage.sprite = sprite;
                break;

            case PlayerPartType.Legs:
                legLeftImage.sprite = sprite;
                legRightImage.sprite = sprite;
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
