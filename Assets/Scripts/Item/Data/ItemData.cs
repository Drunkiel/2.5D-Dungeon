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
}
