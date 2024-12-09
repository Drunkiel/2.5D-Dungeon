using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Skills/Skill data")]
public class SkillData : ScriptableObject
{
    public short ID;
    public string displayedName;
    public string spritePath;
    public string spriteIconPath;
    public string animationName;
    public Vector3 size;
    public Vector3 center;
    public SkillType type;
    public bool worksOnSelf;
    public bool worksOnOthers;
    public List<Attributes> _skillAttributes = new();

    public Sprite GetSprite(string path, float pixelsPerUnit)
    {
        //Create new texture
        byte[] spriteData = File.ReadAllBytes(path);
        Texture2D texture = new(2, 2)
        {
            filterMode = FilterMode.Point
        };

        //Assigning data
        if (texture.LoadImage(spriteData))
        {
            //Convert texture to sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            return sprite;
        }

        return null;
    }
}
