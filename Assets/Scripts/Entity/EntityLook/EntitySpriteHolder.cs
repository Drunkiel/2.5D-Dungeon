using System;
using UnityEngine;

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
