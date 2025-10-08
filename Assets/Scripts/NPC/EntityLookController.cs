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
