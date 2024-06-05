using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public InventorySlot currentSlot;
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / 1.5f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        RectTransform rectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }

    //TODO: Fix why if dropped out of inventory not reseting
}
