using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Collectable data")]
public class CollectableData : ScriptableObject
{
    public ItemData _itemData;
    [JsonIgnore] public Sprite itemSprite;
    [JsonIgnore] public Sprite iconSprite;
}
