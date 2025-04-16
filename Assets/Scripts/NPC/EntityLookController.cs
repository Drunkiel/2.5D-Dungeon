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

public class EntityLookController : SaveLoadSystem
{
    public EntityLook _entityLook;
    public string skinPath;
    public BodyType bodyType;

    private void Start()
    {
        if (_entityLook.headImage != null)
            UpdateEntityLook(EntityPartType.Head, GetSprite($"{skinsSavePath}{skinPath}/head.png", 100f));

        if (_entityLook.bodyImage != null)
            UpdateEntityLook(EntityPartType.Body, GetSprite($"{skinsSavePath}{skinPath}/body.png", 100f));

        if (_entityLook.armLeftImage != null && _entityLook.armRightImage != null)
            UpdateEntityLook(EntityPartType.Arms, GetSprite($"{skinsSavePath}{skinPath}/arm.png", 100f));

        if (_entityLook.handLeftImage != null && _entityLook.handRightImage != null)
            UpdateEntityLook(EntityPartType.Hands, GetSprite($"{skinsSavePath}{skinPath}/hand.png", 100f));

        if (_entityLook.legLeftImage != null && _entityLook.legRightImage != null)
            UpdateEntityLook(EntityPartType.Legs, GetSprite($"{skinsSavePath}{skinPath}/leg.png", 100f));

        UpdateBodyType();
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
        switch(bodyType)
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
}
