using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
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

        if (facingRight)
        {
            //Change parent to original state
            _entityLook.armRightImage.transform.parent.parent.localScale = new(facingCamera ? 1 : -1, 1, 1);

            _entityLook.armRightImage.sortingOrder = 4;
            _entityLook.handRightImage.sortingOrder = 5;

            _entityLook.armLeftImage.sortingOrder = 0;
            _entityLook.handLeftImage.sortingOrder = 1;

            _entityLook.handRightImage.transform.localScale = new(facingCamera ? 0.5f : -0.5f, 0.5f, 0.5f);
            _entityLook.handLeftImage.transform.localScale = new(facingCamera ? -0.5f : 0.5f, 0.5f, 0.5f);

            if (TryGetComponent(out ItemController _itemController))
            {
                if (_itemController._gearHolder._weaponRight != null)
                    _itemController._gearHolder._weaponRight.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 6;
                if (_itemController._gearHolder._weaponLeft != null)
                    _itemController._gearHolder._weaponLeft.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
        }
        else
        {
            //Swap positions
            _entityLook.armRightImage.transform.parent.parent.localScale = new(facingCamera ? -1 : 1, 1, 1);

            _entityLook.armRightImage.sortingOrder = 0;
            _entityLook.handRightImage.sortingOrder = 1;

            _entityLook.armLeftImage.sortingOrder = 4;
            _entityLook.handLeftImage.sortingOrder = 5;

            _entityLook.handRightImage.transform.localScale = new(facingCamera ? -0.5f : 0.5f, 0.5f, 0.5f);
            _entityLook.handLeftImage.transform.localScale = new(facingCamera ? 0.5f : -0.5f, 0.5f, 0.5f);

            if (TryGetComponent(out ItemController _itemController))
            {
                if (_itemController._gearHolder._weaponRight != null)
                    _itemController._gearHolder._weaponRight.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 1;
                if (_itemController._gearHolder._weaponLeft != null)
                    _itemController._gearHolder._weaponLeft.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 6;
            }

        }
    }

    private void UpdateBodyType()
    {
        switch (bodyType)
        {
            case BodyType.Normal:
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
}
