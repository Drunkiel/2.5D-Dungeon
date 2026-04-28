using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityLookController : SaveLoadSystem
{
    public EntityLook _entityLook;
    public EntitySpriteHolder _spriteHolder;
    public string skinPath;
    public BodyType bodyType;
    public Vector2 direction = new Vector2(1, 1);

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void LoadSprites()
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

        LoadSprites();

        bool facingCamera = false;
        if (TryGetComponent(out EntityController _entityController))
            facingCamera = _entityController.isFacingCamera;

        UpdateEntityLookAll(facingCamera);
        UpdateBodyType();
        InventoryController.instance._entityPreview.UpdateAllByEntity(_entityLook, _spriteHolder, GameController.instance._player._itemController._gearHolder);
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
        Vector2 newDirection = new(
            facingRight ? 1 : -1,
            facingCamera ? 1 : -1
        );

        direction = newDirection;

        float x = direction.x;   // 1 or -1
        float y = direction.y;   // 1 or -1

        //Flip entity parts
        _entityLook.armRightImage.transform.parent.parent.localScale =
            new Vector3(x * y, 1, 1);

        _entityLook.handRightImage.transform.localScale =
            new Vector3(x * y, 1, 1);

        _entityLook.handLeftImage.transform.localScale =
            new Vector3(-x * y, 1, 1);

        //Sorting entity parts
        _entityLook.armRightImage.sortingOrder = x == 1 ? 4 : 0;
        _entityLook.handRightImage.sortingOrder = x == 1 ? 5 : 1;
        _entityLook.armLeftImage.sortingOrder = x == 1 ? 0 : 4;
        _entityLook.handLeftImage.sortingOrder = x == 1 ? 1 : 5;

        //Update items
        if (TryGetComponent(out ItemController itemController))
        {
            GearHolder gear = itemController._gearHolder;

            int rightWeaponOrder = x == 1 ? 6 : 1;
            int leftWeaponOrder = x == 1 ? 1 : 6;

            if (gear._weaponRight != null)
                gear._weaponRight.transform.GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().sortingOrder = rightWeaponOrder;

            if (gear._weaponLeft != null)
                gear._weaponLeft.transform.GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().sortingOrder = leftWeaponOrder;

            Sprite headSprite = y == 1 ? gear._armorHead?.itemSpriteFront : gear._armorHead?.itemSpriteBack;
            Sprite chestSprite = y == 1 ? gear._armorChestplate?.itemSpriteFront : gear._armorChestplate?.itemSpriteBack;
            Sprite bootsSprite = y == 1 ? gear._armorBoots?.itemSpriteFront : gear._armorBoots?.itemSpriteBack;

            if (gear._armorHead != null)
                gear._armorHead.transform.GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().sprite = headSprite;

            if (gear._armorChestplate != null)
                gear._armorChestplate.transform.GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().sprite = chestSprite;

            if (gear._armorBoots != null)
            {
                gear._armorBoots.transform.GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().sprite = bootsSprite;

                gear.leftFeetTransform.GetChild(1).GetChild(0).GetChild(0)
                    .GetComponent<SpriteRenderer>().sprite = bootsSprite;
            }
        }

        //Rotate skills to match Entity orientation to camera
        GetComponent<SkillController>()._combatUI.RotateSkills(direction);
        //Update animations
        UpdateAnimationByDirection();
    }

    private void UpdateAnimationByDirection()
    {
        if (anim == null)
            return;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        if (anim.GetCurrentAnimatorClipInfoCount(0) == 0)
            return;

        string currentName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        string baseName = currentName.EndsWith("_R")
            ? currentName.Replace("_R", "")
            : currentName;

        string reversedName = baseName + "_R";

        string targetName = GetCorrectAnimation(direction, baseName, reversedName);

        if (targetName == currentName)
            return;

        //Check if animation exists
        if (GetReversedClip(targetName) == null)
            return;

        float normalizedTime = state.normalizedTime % 1f;

        anim.Play(targetName, 0, normalizedTime);
    }

    public string GetCorrectAnimation(Vector2 direction, string animName, string reversedAnimName)
    {
        return direction.x * direction.y == 1 ? animName : reversedAnimName;
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
                _entityLook.legRightImage.transform.parent.localPosition = new(-0.05f, -0.13f, 0);
                _entityLook.legLeftImage.transform.parent.localPosition = new(0.05f, -0.13f, 0);
                break;

            case BodyType.Muscular:
                _entityLook.headImage.transform.localPosition = new(0, 0.175f, 0);
                _entityLook.armRightImage.transform.parent.parent.localPosition = new(0, 0.1f, 0);
                _entityLook.armRightImage.transform.parent.localPosition = new(-0.13f, 0, 0);
                _entityLook.armLeftImage.transform.parent.localPosition = new(0.13f, 0, 0);
                _entityLook.legRightImage.transform.parent.localPosition = new(-0.05f, -0.13f, 0);
                _entityLook.legLeftImage.transform.parent.localPosition = new(0.05f, -0.13f, 0);
                break;

            case BodyType.Tall:
                _entityLook.headImage.transform.localPosition = new(0, 0.2f, 0);
                _entityLook.armRightImage.transform.parent.parent.localPosition = new(0, 0.125f, 0);
                _entityLook.legRightImage.transform.parent.localPosition = new(-0.05f, -0.13f, 0);
                _entityLook.legLeftImage.transform.parent.localPosition = new(0.05f, -0.13f, 0);
                break;
        }
    }

    public AnimationClip GetReversedClip(string reversedName)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            if (clip.name == reversedName)
                return clip;
        }

        return null;
    }

    public void ApplySkinInEditor()
    {
        LoadSprites();
        UpdateEntityLookAll(true);
        UpdateBodyType();

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        if (_entityLook != null)
        {
            if (_entityLook.headImage != null)
                EditorUtility.SetDirty(_entityLook.headImage);

            if (_entityLook.bodyImage != null)
                EditorUtility.SetDirty(_entityLook.bodyImage);
        }
#endif
    }
}
