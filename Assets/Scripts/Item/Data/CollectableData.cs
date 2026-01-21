using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Weapon data")]
public class CollectableData : ScriptableObject
{
    public ItemData _itemData;
    public Sprite itemSprite;
    public Sprite iconSprite;
}
