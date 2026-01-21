using System;
using UnityEngine;

public enum EntityPartType
{
    Head,
    Body,
    Arms,
    Hands,
    Legs
}

public enum BodyType
{
    Normal,
    Muscular,
    Tall,
}

[Serializable]
public class EntitySpriteHolder
{
    [Header("Front")]
    public Sprite head_Front;
    public Sprite body_Front;
    public Sprite arm_Front;
    public Sprite hand_Front;
    public Sprite leg_Front;

    [Header("Back")]
    public Sprite head_Back;
    public Sprite body_Back;
    public Sprite arm_Back;
    public Sprite hand_Back;
    public Sprite leg_Back;
}

public class EntityLookController : SaveLoadSystem
{
    public EntityLook _entityLook;
    public EntitySpriteHolder _spriteHolder;
    public string skinPath;
    public BodyType bodyType;
    public bool isRotated;

    private void Start()
    {
        LoadSpirtes();
        UpdateEntityLookAll(true);
        UpdateBodyType();
    }

    private void LoadSpirtes()
    {
        string path = $"{skinsSavePath}{skinPath}";

        _spriteHolder.head_Front = GetSprite($"{path}/head_front.png", 100f);
        _spriteHolder.body_Front = GetSprite($"{path}/body_front.png", 100f);
        _spriteHolder.arm_Front = GetSprite($"{path}/arm_front.png", 100f);
        _spriteHolder.hand_Front = GetSprite($"{path}/hand_front.png", 100f);
        _spriteHolder.leg_Front = GetSprite($"{path}/leg_front.png", 100f);

        _spriteHolder.head_Back = GetSprite($"{path}/head_back.png", 100f);
        _spriteHolder.body_Back = GetSprite($"{path}/body_back.png", 100f);
        _spriteHolder.arm_Back = GetSprite($"{path}/arm_back.png", 100f);
        _spriteHolder.hand_Back = GetSprite($"{path}/hand_back.png", 100f);
        _spriteHolder.leg_Back = GetSprite($"{path}/leg_back.png", 100f);
    }

    public void SpriteLoader(string newPath = null, string bodyType = null)
    {
        if (!string.IsNullOrEmpty(newPath))
            skinPath = newPath;

        if (!string.IsNullOrEmpty(bodyType))
            this.bodyType = Enum.Parse<BodyType>(bodyType);

        LoadSpirtes();

        bool facingCamera = false;
        if (TryGetComponent(out EntityController _entityController))
            facingCamera = _entityController.isFacingCamera;

        UpdateEntityLookAll(facingCamera);
        UpdateBodyType();
        InventoryController.instance._entityPreview.UpdateAllByEntity(_entityLook, _spriteHolder, GameController.instance._player._holdingController._itemController._gearHolder);
    }

    public void UpdateEntityLookAll(bool isFacingCamera)
    {
        string facing = isFacingCamera ? "_front" : "_back";

        if (_entityLook.headImage != null)
            UpdateEntityLook(EntityPartType.Head, GetSprite($"{skinsSavePath}{skinPath}/head{facing}.png", 100f));

        if (_entityLook.bodyImage != null)
            UpdateEntityLook(EntityPartType.Body, GetSprite($"{skinsSavePath}{skinPath}/body{facing}.png", 100f));

        if (_entityLook.armLeftImage != null && _entityLook.armRightImage != null)
            UpdateEntityLook(EntityPartType.Arms, GetSprite($"{skinsSavePath}{skinPath}/arm{facing}.png", 100f));

        if (_entityLook.handLeftImage != null && _entityLook.handRightImage != null)
            UpdateEntityLook(EntityPartType.Hands, GetSprite($"{skinsSavePath}{skinPath}/hand{facing}.png", 100f));

        if (_entityLook.legLeftImage != null && _entityLook.legRightImage != null)
            UpdateEntityLook(EntityPartType.Legs, GetSprite($"{skinsSavePath}{skinPath}/leg{facing}.png", 100f));
    }

    public void UpdateEntityLook(EntityPartType partType, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case EntityPartType.Head:
                _entityLook.headImage.sprite = sprite;
                break;

            case EntityPartType.Body:
                _entityLook.bodyImage.sprite = sprite;
                break;

            case EntityPartType.Arms:
                _entityLook.armLeftImage.sprite = sprite;
                _entityLook.armRightImage.sprite = sprite;
                break;

            case EntityPartType.Hands:
                _entityLook.handLeftImage.sprite = sprite;
                _entityLook.handRightImage.sprite = sprite;
                break;

            case EntityPartType.Legs:
                _entityLook.legLeftImage.sprite = sprite;
                _entityLook.legRightImage.sprite = sprite;
                break;
        }
    }

    public void RotateCharacter(bool facingRight, bool facingCamera)
    {
        isRotated = facingRight;

        int rightArmOrder = facingRight ? 4 : 0;
        int rightHandOrder = facingRight ? 5 : 1;
        int leftArmOrder = facingRight ? 0 : 4;
        int leftHandOrder = facingRight ? 1 : 5;

        float scaleSign = facingCamera ? 1f : -1f;
        float flipDirection = facingRight ? scaleSign : -scaleSign;

        _entityLook.armRightImage.transform.parent.parent.localScale = new Vector3(flipDirection, 1, 1);

        _entityLook.armRightImage.sortingOrder = rightArmOrder;
        _entityLook.handRightImage.sortingOrder = rightHandOrder;
        _entityLook.armLeftImage.sortingOrder = leftArmOrder;
        _entityLook.handLeftImage.sortingOrder = leftHandOrder;

        float handScaleRight = facingRight ? scaleSign * 1f : -scaleSign * 1f;
        float handScaleLeft = facingRight ? -scaleSign * 1f : scaleSign * 1f;

        _entityLook.handRightImage.transform.localScale = new Vector3(handScaleRight, 1f, 1f);
        _entityLook.handLeftImage.transform.localScale = new Vector3(handScaleLeft, 1f, 1f);

        if (TryGetComponent(out ItemController itemController))
        {
            GearHolder _gearHolder = itemController._gearHolder;
            int rightWeaponOrder = facingRight ? 6 : 1;
            int leftWeaponOrder = facingRight ? 1 : 6;

            if (_gearHolder._weaponRight != null)
                _gearHolder._weaponRight.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = rightWeaponOrder;

            if (_gearHolder._weaponLeft != null)
                _gearHolder._weaponLeft.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = leftWeaponOrder;

            if (_gearHolder._armorHead != null)
                _gearHolder._armorHead.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite =
                facingCamera ?
                _gearHolder._armorHead.itemSpriteFront :
                _gearHolder._armorHead.itemSpriteBack;

            if (_gearHolder._armorChestplate != null)
                _gearHolder._armorChestplate.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite =
                facingCamera ?
                _gearHolder._armorChestplate.itemSpriteFront :
                _gearHolder._armorChestplate.itemSpriteBack;

            if (_gearHolder._armorBoots != null)
            {
                _gearHolder._armorBoots.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite =
                facingCamera ?
                _gearHolder._armorBoots.itemSpriteFront :
                _gearHolder._armorBoots.itemSpriteBack;

                _gearHolder.leftFeetTransform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite =
                facingCamera ?
                _gearHolder._armorBoots.itemSpriteFront :
                _gearHolder._armorBoots.itemSpriteBack;
            }
        }
    }

    private void UpdateBodyType()
    {
        switch (bodyType)
        {
            case BodyType.Normal:
                _entityLook.headImage.transform.localPosition = new(0, 0.15f, 0);
                _entityLook.armRightImage.transform.parent.parent.localPosition = new(0, 0.075f, 0);
                _entityLook.armRightImage.transform.parent.localPosition = new(-0.1f, 0, 0);
                _entityLook.armLeftImage.transform.parent.localPosition = new(0.1f, 0, 0);
                break;

            case BodyType.Muscular:
                _entityLook.headImage.transform.localPosition = new(0, 0.175f, 0);
                _entityLook.armRightImage.transform.parent.parent.localPosition = new(0, 0.1f, 0);
                _entityLook.armRightImage.transform.parent.localPosition = new(-0.13f, 0, 0);
                _entityLook.armLeftImage.transform.parent.localPosition = new(0.13f, 0, 0);
                break;

            case BodyType.Tall:
                _entityLook.headImage.transform.localPosition = new(0, 0.2f, 0);
                _entityLook.armRightImage.transform.parent.parent.localPosition = new(0, 0.125f, 0);
                break;
        }
    }
}
