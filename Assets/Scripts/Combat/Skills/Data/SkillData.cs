using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Skills/Skill data")]
public class SkillData : ScriptableObject
{
    public short skillID;
    public string skillName;
    public Sprite skillSprite;
    public Sprite skillIconSprite;
    public List<Attributes> _skillAttributes = new();
}
