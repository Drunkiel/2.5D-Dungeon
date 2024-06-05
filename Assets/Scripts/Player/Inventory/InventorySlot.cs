using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotID;
    public Image image;
    public ItemID _itemID;
    public GameObject itemPlacePrefab;

    public void AssignItem()
    {
        Sprite sprite = null;
        switch (_itemID.itemType)
        {
            case ItemType.Weapon:
                sprite = _itemID._weaponItem._itemData.itemSprite;
                break;
            case ItemType.Armor:
                sprite = _itemID._armorItem._itemData.itemSprite;
                break;
            case ItemType.Collectable:
                sprite = _itemID._armorItem._itemData.itemSprite;
                break;
        }

        image.sprite = sprite;
    }

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform rectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
        rectTransform.GetComponent<DragDrop>().currentSlot = this;
        rectTransform.SetParent(rectTransform.GetComponent<DragDrop>().currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}
