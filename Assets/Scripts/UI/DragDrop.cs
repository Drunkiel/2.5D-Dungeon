using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public InventorySlot currentSlot;
    public Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentSlot._itemID = null;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / 1.5f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
        currentSlot._itemID = transform.GetChild(1).GetComponent<ItemID>();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentSlot == null)
            currentSlot = eventData.pointerEnter.transform.parent.parent.GetComponent<InventorySlot>();

        rectTransform.SetParent(InventoryController.instance.slotParent.parent);
    }

    public void OnDrop(PointerEventData eventData)
    {
        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}
