using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerLook
{
    [Header("Head")]
    public SpriteRenderer headImage;

    [Header("Body")]
    public SpriteRenderer bodyImage;

    [Header("Arms")]
    public SpriteRenderer armLeftImage;
    public SpriteRenderer armRightImage;

    [Header("Hands")]
    public SpriteRenderer handLeftImage;
    public SpriteRenderer handRightImage;

    [Header("Legs")]
    public SpriteRenderer legLeftImage;
    public SpriteRenderer legRightImage;
}