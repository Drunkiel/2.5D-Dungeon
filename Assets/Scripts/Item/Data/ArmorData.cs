using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Armor data")]
public class ArmorData : ScriptableObject
{
    public ItemData _itemData;
    public ArmorType armorType;
    [JsonIgnore] public Sprite itemSpriteFront;
    [JsonIgnore] public Sprite itemSpriteBack;
    [JsonIgnore] public Sprite iconSprite;
}
