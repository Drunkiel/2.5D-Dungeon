using UnityEngine;

public enum PlayerPartType
{
    Head,
    Body,
    Arms,
    Hands,
    Legs
}

public class PlayerLookController : SaveLoadSystem
{
    [SerializeField] private PlayerLook _playerLook;
    public string skinPath;

    private void Start()
    {
        UpdatePlayerLook(PlayerPartType.Head, GetSprite($"{skinsSavePath}{skinPath}/head.png", 100f));
        UpdatePlayerLook(PlayerPartType.Body, GetSprite($"{skinsSavePath}{skinPath}/body.png", 100f));
        UpdatePlayerLook(PlayerPartType.Arms, GetSprite($"{skinsSavePath}{skinPath}/arm.png", 100f));
        UpdatePlayerLook(PlayerPartType.Hands, GetSprite($"{skinsSavePath}{skinPath}/hand.png", 100f));
        UpdatePlayerLook(PlayerPartType.Legs, GetSprite($"{skinsSavePath}{skinPath}/leg.png", 100f));
    }

    public void UpdatePlayerLook(PlayerPartType partType, Sprite sprite)
    {
        if (sprite == null)
            return;

        switch (partType)
        {
            case PlayerPartType.Head:
                _playerLook.headImage.sprite = sprite;
                break;
            
            case PlayerPartType.Body:
                _playerLook.bodyImage.sprite = sprite;
                break;
            
            case PlayerPartType.Arms:
                _playerLook.armLeftImage.sprite = sprite;
                _playerLook.armRightImage.sprite = sprite;
                break;

            case PlayerPartType.Hands:
                _playerLook.handLeftImage.sprite = sprite;
                _playerLook.handRightImage.sprite = sprite;
                break;

            case PlayerPartType.Legs:
                _playerLook.legLeftImage.sprite = sprite;
                _playerLook.legRightImage.sprite = sprite;
                break;
        }
    }
}
