using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Item data")]
public class ItemData : ScriptableObject
{
    public short ID;
    public string displayedName;
    public ItemType itemType;
    public string spritePath;
    public string spriteIconPath;
    public List<Attributes> _attributes = new();
    public List<ItemBuff> _itemBuffs = new();

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
