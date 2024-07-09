using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotID;
    public ItemID _itemID;
    public GameObject itemPlacePrefab;

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform rectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
        rectTransform.GetComponent<DragDrop>().currentSlot = this;
        rectTransform.SetParent(rectTransform.GetComponent<DragDrop>().currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}
