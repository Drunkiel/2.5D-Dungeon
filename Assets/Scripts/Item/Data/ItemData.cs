using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Items/Item data")]
public class ItemData : ScriptableObject
{
    public short itemID;
    public string itemName;
    public Sprite itemSprite;
    public Sprite itemIconSprite;
}
