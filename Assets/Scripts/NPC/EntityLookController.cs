using UnityEngine;

public enum EntityPartType
{
    Head,
    Body,
    Arms,
    Hands,
    Legs
}

public class EntityLookController : SaveLoadSystem
{
    public EntityLook _entityLook;
    public string skinPath;

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
}
