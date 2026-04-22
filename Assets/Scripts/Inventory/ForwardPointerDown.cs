using UnityEngine;
using UnityEngine.EventSystems;

public class ForwardPointerDown : MonoBehaviour, IPointerDownHandler
{
    private DragDropSlot slot;

    void Start()
    {
        slot = transform.parent.GetComponent<DragDropSlot>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        slot.OnPointerDown(eventData);
    }
}